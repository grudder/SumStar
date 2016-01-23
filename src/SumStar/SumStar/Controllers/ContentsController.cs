using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity.Owin;

using Newtonsoft.Json;

using PagedList;

using SumStar.DataAccess;
using SumStar.Helper;
using SumStar.Models;
using SumStar.Services;

namespace SumStar.Controllers
{
	public class ContentsController : Controller
	{
		private ApplicationDbContext _dbContext;
		private readonly CategoryService _categoryService;
		private readonly ContentService _contentService;

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

		public ContentService ContentService => _contentService ?? new ContentService(DbContext);

		public ContentsController()
		{
		}

		public ContentsController(ApplicationDbContext dbContext)
		{
			DbContext = dbContext;
			_categoryService = new CategoryService(DbContext);
			_contentService = new ContentService(DbContext);
		}

		[Authorize(Roles = "ContentAdmin")]
		// GET: Contents?categoryId=5
		public ActionResult Index(int? categoryId)
		{
			if (categoryId == null)
			{
				categoryId = 1;
			}
			ViewBag.CategoryId = categoryId;

			return View();
		}

		// GET: Contents/GetJsonByCategory?categoryId=5
		public ActionResult GetJsonByCategory(int categoryId)
		{
			Expression<Func<Content, bool>> predicate = i => i.CategoryId == categoryId;
			IList<Category> categories = CategoryService.GetChilds(categoryId, true);
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

		// GET: Contents/List?categoryId=5&categoryName=新闻动态&pageSize=20&page=1
		public ActionResult List(int? categoryId, string categoryName, int? pageSize, int? page)
		{
			Category category;
			if (categoryId == null)
			{
				category = DbContext.Categories.First(i => i.Name == categoryName);
			}
			else
			{
				category = DbContext.Categories.Find(categoryId);
			}
			if (category == null)
			{
				return HttpNotFound();
			}
			ViewBag.Category = category;
			ViewBag.Level1Category = CategoryService.GetLevel1Category(category);

			ActionResult actionResult = new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			switch (category.DisplayMode)
			{
				case CategoryDisplayMode.TitleList:
					IPagedList<Content> pagedList = ContentService.GetByCategory(category.Id, pageSize, page);
                    actionResult = View(pagedList);
					break;

				case CategoryDisplayMode.ImageList:
					actionResult = RedirectToAction("List", "ArticleContents", new {categoryId = category.Id});
					break;

				case CategoryDisplayMode.FirstContentDetail:
					Content content = ContentService.GetFirstContentByCategory(category.Id);
					actionResult = RedirectToAction("Detail", "Contents", new {content.Id});
					break;
			}

			ViewBag.IsEnglish = category.IsEnglish;
			return actionResult;
		}

		[Authorize]
		// GET: Contents/List?categoryId=5&categoryName=新闻动态&pageSize=20&page=1
		public ActionResult IntranetList(int? categoryId, string categoryName, int? pageSize, int? page)
		{
			return RedirectToAction("List", "Contents", new {categoryId, categoryName, pageSize, page});
		}

		// GET: Contents/Detail/5
		public ActionResult Detail(int? id)
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
			return RedirectToAction("Detail", controllerName, new {id});
		}

		[Authorize(Roles = "ContentAdmin")]
		// GET: Contents/Create?categoryId=5
		public ActionResult Create(int categoryId)
		{
			Category category = DbContext.Categories.Find(categoryId);
			if (category == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			string controllerName = category.ContentType + "Contents";
			return RedirectToAction("Create", controllerName, new
			{
				categoryId
			});
		}

		[Authorize(Roles = "ContentAdmin")]
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
			return RedirectToAction("Edit", controllerName, new
			{
				id
			});
		}

		[Authorize(Roles = "ContentAdmin")]
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

			return RedirectToAction("Delete", controllerName, new
			{
				id
			});
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
