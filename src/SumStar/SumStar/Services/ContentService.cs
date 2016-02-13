using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using PagedList;

using SumStar.DataAccess;
using SumStar.Helper;
using SumStar.Models;

namespace SumStar.Services
{
	public class ContentService
	{
		private readonly ApplicationDbContext _dbContext;

		private readonly CategoryService _categoryService;

		public ContentService(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
			_categoryService = new CategoryService(dbContext);
		}

		/// <summary>
		/// 获取栏目下的前几条内容。
		/// </summary>
		/// <param name="categoryId">栏目标识。</param>
		/// <param name="topN">前几条。</param>
		/// <returns>栏目下的前几条内容。</returns>
		public IList<Content> GetTopContentsByCategory(int categoryId, int topN = 5)
		{
			var query = from content in _dbContext.Contents
						where content.CategoryId == categoryId
						orderby content.DisplayOrder, content.CreateTime descending
						select content;
			return query.Take(topN).ToList();
		}

		/// <summary>
		/// 获取栏目下的分页内容。
		/// </summary>
		/// <param name="categoryId">栏目标识。</param>
		/// <param name="pageSize">页面大小。</param>
		/// <param name="pageIndex">页面索引。</param>
		/// <returns>栏目下的内容。</returns>
		public IPagedList<Content> GetByCategory(int categoryId, int? pageSize, int? pageIndex)
		{
			Expression<Func<Content, bool>> predicate = i => i.CategoryId == categoryId;
			IList<Category> categories = _categoryService.GetChilds(categoryId, true);
			predicate = categories.Aggregate(predicate, (current, c) => current.Or(i => i.CategoryId == c.Id));

			var query = from content in _dbContext.Contents
						orderby content.DisplayOrder descending, content.CreateTime descending
						select content;
			// 分页处理
			pageSize = (pageSize ?? 15);
			pageIndex = (pageIndex ?? 1);
			IPagedList<Content> pagedList = query.Where(predicate).ToPagedList(pageIndex.Value, pageSize.Value);
			return pagedList;
		}

		/// <summary>
		/// 获取栏目下的分页图片内容。
		/// </summary>
		/// <param name="categoryId">栏目标识。</param>
		/// <param name="pageSize">页面大小。</param>
		/// <param name="pageIndex">页面索引。</param>
		/// <returns>栏目下的图片内容。</returns>
		public IPagedList<ArticleContent> GetArticleContentsByCategory(int categoryId, int? pageSize, int? pageIndex)
		{
			Expression<Func<ArticleContent, bool>> predicate = i => i.CategoryId == categoryId;
			IList<Category> categories = _categoryService.GetChilds(categoryId, true);
			predicate = categories.Aggregate(predicate, (current, c) => current.Or(i => i.CategoryId == c.Id));

			var query = from content in _dbContext.ArticleContents
						orderby content.DisplayOrder descending, content.CreateTime descending
						select content;
			// 分页处理
			pageSize = (pageSize ?? 15);
			pageIndex = (pageIndex ?? 1);
			IPagedList<ArticleContent> pagedList = query.Where(predicate).ToPagedList(pageIndex.Value, pageSize.Value);
			return pagedList;
		}

		/// <summary>
		/// 获取栏目下的内容。
		/// </summary>
		/// <param name="categoryName">栏目名称。</param>
		/// <returns>栏目下的内容。</returns>
		public IList<Content> GetByCategory(string categoryName)
		{
			return _dbContext.Contents.Where(c => c.Category.Name == categoryName).ToList();
		}

		/// <summary>
		/// 获取栏目下的第一条内容。
		/// </summary>
		/// <param name="categoryId">栏目标识。</param>
		/// <returns>栏目下的第一条内容。</returns>
		public Content GetFirstContentByCategory(int categoryId)
		{
			IList<Category> childCategories = _categoryService.GetChilds(categoryId);
			if (childCategories.Count > 0)
			{
				// 如果有子栏目，则获取子栏目中的第一个
				categoryId = childCategories[0].Id;
			}
			var query = from content in _dbContext.Contents
						where content.CategoryId == categoryId
						orderby content.DisplayOrder, content.CreateTime descending
						select content;
			return query.Take(1).SingleOrDefault();
		}
	}
}
