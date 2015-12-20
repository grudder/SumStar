using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity.Owin;

using Newtonsoft.Json;

using SumStar.DataAccess;
using SumStar.Helper;
using SumStar.Models;
using SumStar.Services;

namespace SumStar.Controllers
{
	[Authorize(Roles = "ContentAdmin")]
	public class ContentsController : Controller
	{
		private ApplicationDbContext _dbContext;
		private readonly CategoryService _categoryService;

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

		public ContentsController()
		{
		}

		public ContentsController(ApplicationDbContext dbContext)
		{
			DbContext = dbContext;
			_categoryService = new CategoryService(DbContext);
		}

		// GET: Contents?categoryId=5
		public ActionResult Index(int? categoryId)
		{
			Category category = DbContext.Categories.Find(categoryId);
			ViewBag.ContentType = category?.ContentType;

			return View();
		}

		// GET: Contents/GetByCategory?categoryId=5
		public ActionResult GetByCategory(int categoryId)
		{
			Expression<Func<Content, bool>> predicate = i => i.CategoryId == categoryId;
			IEnumerable<Category> categories = CategoryService.GetRecursiveChilds(categoryId);
			predicate = categories.Aggregate(predicate, (current, category) => current.Or(i => i.CategoryId == category.Id));

			var data = TableDataSource<Content>.FromRequest(HttpContext.Request, DbContext.Contents, predicate);
			var json = JsonConvert.SerializeObject(
				data,
				new JsonSerializerSettings
				{
					NullValueHandling = NullValueHandling.Ignore
				});

			return Content(json);
		}

		// GET: Contents/Create?categoryId=5
		public ActionResult Create(int categoryId)
		{
			Category category = DbContext.Categories.Find(categoryId);
			if (category == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			string controllerName = category.ContentType + "Contents";
			
			return RedirectToAction("Create", controllerName, new {categoryId});
		}

		// GET: Contents/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Content content = DbContext.Contents.Find(id);
			if (content == null)
			{
				return HttpNotFound();
			}
			if (content.Category == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			string controllerName = content.Category.ContentType + "Contents";
			
			return RedirectToAction("Edit", controllerName, new {id});
		}

		// GET: Contents/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Content content = DbContext.Contents.Find(id);
			if (content == null)
			{
				return HttpNotFound();
			}
			if (content.Category == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			string controllerName = content.Category.ContentType + "Contents";
			
			return RedirectToAction("Delete", controllerName, new {id});
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
