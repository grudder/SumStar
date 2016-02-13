using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using Newtonsoft.Json;

using SumStar.DataAccess;
using SumStar.Helper;
using SumStar.Models;
using SumStar.Models.ViewModels;
using SumStar.Services;

namespace SumStar.Controllers
{
	public class CategoriesController : Controller
	{
		private ApplicationDbContext _dbContext;

		private readonly CategoryService _categoryService;

		public CategoriesController()
		{
		}

		public CategoriesController(ApplicationDbContext dbContext)
		{
			DbContext = dbContext;
			_categoryService = new CategoryService(DbContext);
		}

		public ApplicationDbContext DbContext
		{
			get
			{
				return _dbContext ?? HttpContext.GetOwinContext().Get<ApplicationDbContext>();
			}
			private set
			{
				_dbContext = value;
			}
		}

		public CategoryService CategoryService => _categoryService ?? new CategoryService(DbContext);

		// GET: Categories
		[Authorize(Roles = "ContentAdmin")]
		public ActionResult Index()
		{
			return View();
		}

		// GET: Categories/NavigatorPartial/5
		public PartialViewResult NavigatorPartial(int id)
		{
			IList<Category> categories = CategoryService.GetRecursiveParents(id, true);

			Category category = categories.Last();
			ViewBag.IsEnglish = category.IsEnglish;

			return PartialView("_NavigatorPartial", categories);
		}

		// GET: Categories/TreePanelPartial/5
		public PartialViewResult TreePanelPartial(Category category, Category level1Category)
		{
			IList<Category> categories = CategoryService.GetChilds(level1Category.Id);
			ViewBag.Level1Category = level1Category;
            return PartialView("_TreePanelPartial", categories);
		}

		// GET: Categories/GetBootstrapTree/5?archor=treeCategory
		public ActionResult GetBootstrapTree(int id, string archor)
		{
			IList<BootstrapTreeNode> treeNodes = CategoryService.GetBootstrapTreeNodes(id, archor);
			string json = JsonConvert.SerializeObject(
				treeNodes,
				new JsonSerializerSettings
				{
					NullValueHandling = NullValueHandling.Ignore
				});

			return Content(json);
		}

		// GET: Categories/GetTree
		public ActionResult GetTree()
		{
			IList<ZTreeNode> treeNodes = CategoryService.GetChildTreeNodes(null);
			string json = JsonConvert.SerializeObject(
				treeNodes,
				new JsonSerializerSettings
				{
					NullValueHandling = NullValueHandling.Ignore
				});

			return Content(json);
		}

		// GET: Categories/GetChildTreeNodes/5
		public ActionResult GetChildTreeNodes(int? id)
		{
			IList<ZTreeNode> treeNodes = CategoryService.GetChildTreeNodes(id);
			string json = JsonConvert.SerializeObject(
				treeNodes,
				new JsonSerializerSettings
				{
					NullValueHandling = NullValueHandling.Ignore
				});

			return Content(json);
		}

		// GET: Categories/GetChildCategories/5
		public ActionResult GetChildCategories(int id)
		{
			Expression<Func<Category, bool>> predicate = i => i.ParentId == id;
			var data = TableDataSource<Category>.FromRequest(HttpContext.Request, DbContext.Categories, predicate);
			string json = JsonConvert.SerializeObject(
				data,
				new JsonSerializerSettings
				{
					NullValueHandling = NullValueHandling.Ignore
				});

			return Content(json);
		}

		// GET: Categories/Create/5
		[Authorize(Roles = "ContentAdmin")]
		public ActionResult Create(int? id)
		{
			Category parentCategory = DbContext.Categories.Find(id);
            var category = new Category
			{
				ParentId = id,
				Parent = parentCategory,
				DisplayMode = parentCategory?.DisplayMode
			};
			
			return View(category);
		}
		
		// POST: Categories/Create
		// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
		// 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
		[Authorize(Roles = "ContentAdmin")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(
			[Bind(Include = "Id,ParentId,DisplayOrder,IsEnglish,Name,ContentType,DisplayMode,Remark")] Category category)
		{
			category.IsLeaf = true;
			category.CreateBy = HttpContext.User.Identity.GetUserId();
			category.CreateTime = DateTime.Now;

			if (ModelState.IsValid)
			{
				category.Parent = DbContext.Categories.SingleOrDefault(i => i.Id == category.ParentId);
				DbContext.Categories.Add(category);
				DbContext.SaveChanges();
				return RedirectToAction("Index");
			}
			
			return View(category);
		}

		// GET: Categories/Edit/5
		[Authorize(Roles = "ContentAdmin")]
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var category = DbContext.Categories.Find(id);
			if (category == null)
			{
				return HttpNotFound();
			}
			
			return View(category);
		}

		// POST: Categories/Edit/5
		// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
		// 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
		[Authorize(Roles = "ContentAdmin")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(
			[Bind(Include = "Id,ParentId,DisplayOrder,IsLeaf,IsEnglish,Name,ContentType,DisplayMode,Remark,CreateBy,CreateTime")] Category category)
		{
			if (ModelState.IsValid)
			{
				DbContext.Entry(category).State = EntityState.Modified;
				DbContext.SaveChanges();
				return RedirectToAction("Index");
			}
			
			return View(category);
		}

		// GET: Categories/Delete/5
		[Authorize(Roles = "ContentAdmin")]
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var category = DbContext.Categories.Find(id);
			if (category == null)
			{
				return HttpNotFound();
			}
			return View(category);
		}

		// POST: Categories/Delete/5
		[Authorize(Roles = "ContentAdmin")]
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			var category = DbContext.Categories.Find(id);
			DbContext.Categories.Remove(category);
			DbContext.SaveChanges();
			return RedirectToAction("Index");
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_dbContext != null)
				{
					_dbContext.Dispose();
					_dbContext = null;
				}
			}
			base.Dispose(disposing);
		}
	}
}