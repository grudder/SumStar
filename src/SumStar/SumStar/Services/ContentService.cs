using System.Collections.Generic;
using System.Linq;

using SumStar.DataAccess;
using SumStar.Models;

namespace SumStar.Services
{
	public class ContentService
	{
		private readonly ApplicationDbContext _dbContext;

		public ContentService(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		/// <summary>
		/// 获取栏目下的内容。
		/// </summary>
		/// <param name="categoryId">栏目标识。</param>
		/// <returns>栏目下的内容。</returns>
		public IList<Content> GetByCategory(int categoryId)
		{
			var query = from content in _dbContext.Contents
						where content.CategoryId == categoryId
						orderby content.DisplayOrder
						select content;
			return query.ToList();
		}

		/// <summary>
		/// 获取栏目下的内容。
		/// </summary>
		/// <param name="categoryName">栏目名称。</param>
		/// <returns>栏目下的内容。</returns>
		public IList<Content> GetByCategory(string categoryName)
		{
			var query = from content in _dbContext.Contents
						where content.Category.Name == categoryName
						orderby content.DisplayOrder
						select content;
			return query.ToList();
		}
	}
}
