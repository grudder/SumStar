using System.Collections.Generic;
using System.Linq;

using SumStar.DataAccess;
using SumStar.Models.ViewModels;

namespace SumStar.Services
{
	public class CategoryService
	{
		private readonly ApplicationDbContext _dbContext;

		public CategoryService(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public List<ZTreeNode> GetChildTreeNodes(int? categoryId, string controller = "Categories", string action = "List")
		{
			if (categoryId == null)
			{
				var rootNode = new ZTreeNode
				{
					Id = 0,
					Name = "【所有栏目】",
					Href = $"/{controller}/{action}/{0}",
					IsParent = true,
					Open = true
				};

				return new List<ZTreeNode> { rootNode };
			}

			var query = from category in _dbContext.Categories
						where category.ParentId == categoryId
						orderby category.DisplayOrder
						select new ZTreeNode
						{
							Id = category.Id,
							Name = category.Name,
							IsParent = true
						};
			var treeNodes = query.ToList();
			foreach (ZTreeNode treeNode in treeNodes)
			{
				treeNode.Href = $"/{controller}/{action}/{treeNode.Id}";
			}

			return treeNodes;
		}
	}
}
