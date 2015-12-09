using System;
using System.Collections;
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
		public ActionResult Index()
		{
			return View();
		}

		// GET: Categories/GetChildTreeNodes/5
		public ActionResult GetChildTreeNodes(int? id, string controller = "Categories", string action = "List")
		{
			IList<ZTreeNode> treeNodes = CategoryService.GetChildTreeNodes(id, controller, action);
			string json = JsonConvert.SerializeObject(
				treeNodes,
				new JsonSerializerSettings
				{
					NullValueHandling = NullValueHandling.Ignore
				});

			return Content(json);
		}

		// GET: Categories/GetChildCategories/5
		public ActionResult GetChildCategories(int? id)
		{
			if (id == 0)
			{
				id = null;
			}
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
		public ActionResult Create(int? id)
		{
			var allCategories = DbContext.Categories.ToList();
			var topCategory = new Category
			{
				Id = 0,
				Name = "【网站栏目】"
			};
			allCategories.Insert(0, topCategory);
			ViewBag.ParentId = new SelectList(allCategories, "Id", "Name");
			
			var category = new Category
			{
				ParentId = id
			};
			return View(category);
		}

		// POST: Categories/Create
		// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
		// 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(
			[Bind(Include = "Id,ParentId,DisplayOrder,Name,ContentType,Remark")] Category category)
		{
			category.CreateBy = HttpContext.User.Identity.GetUserId();
			category.CreateTime = DateTime.Now;

			if (ModelState.IsValid)
			{
				if (category.ParentId == 0)
				{
					category.ParentId = null;
				}
				DbContext.Categories.Add(category);
				DbContext.SaveChanges();
				return RedirectToAction("Index");
			}

			var allCategories = DbContext.Categories.ToList();
			var topCategory = new Category
			{
				Id = 0,
				Name = "【网站栏目】"
			};
			allCategories.Insert(0, topCategory);
			ViewBag.ParentId = new SelectList(allCategories, "Id", "Name", category.ParentId);
			return View(category);
		}

		// GET: Categories/Edit/5
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
			ViewBag.CreateBy = new SelectList(DbContext.Users, "Id", "Remark", category.CreateBy);
			ViewBag.ParentId = new SelectList(DbContext.Categories, "Id", "Name", category.ParentId);
			return View(category);
		}

		// POST: Categories/Edit/5
		// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
		// 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(
			[Bind(Include = "Id,ParentId,DisplayOrder,Name,ContentType,Remark")] Category category)
		{
			if (ModelState.IsValid)
			{
				DbContext.Entry(category).State = EntityState.Modified;
				DbContext.SaveChanges();
				return RedirectToAction("Index");
			}
			ViewBag.CreateBy = new SelectList(DbContext.Users, "Id", "Remark", category.CreateBy);
			ViewBag.ParentId = new SelectList(DbContext.Categories, "Id", "Name", category.ParentId);
			return View(category);
		}

		// GET: Categories/Delete/5
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