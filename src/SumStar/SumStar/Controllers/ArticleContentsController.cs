using System;
using System.Data.Entity;
using System.Net;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using SumStar.DataAccess;
using SumStar.Models;
using SumStar.Services;

namespace SumStar.Controllers
{
	public class ArticleContentsController : Controller
	{
		private readonly CategoryService _categoryService;
		private ApplicationDbContext _dbContext;

		public ArticleContentsController()
		{
		}

		public ArticleContentsController(ApplicationDbContext dbContext)
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

		// GET: ArticleContents/Create?categoryId=5
		public ActionResult Create(int categoryId)
		{
			ViewBag.CategoryId = new SelectList(DbContext.Categories, "Id", "Name", categoryId);

			return View();
		}

		// POST: ArticleContents/Create?categoryId=5
		// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
		// 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateInput(false)]
		public ActionResult Create([Bind(Include = "Id,CategoryId,Title,DisplayOrder,TopicImage,Author,Content")] ArticleContent articleContent)
		{
			articleContent.CreateBy = HttpContext.User.Identity.GetUserId();
			articleContent.CreateTime = DateTime.Now;

			if (ModelState.IsValid)
			{
				DbContext.Contents.Add(articleContent);
				DbContext.SaveChanges();
				return RedirectToAction("Index", "Contents", new {categoryId = articleContent.CategoryId});
			}

			ViewBag.CategoryId = new SelectList(DbContext.Categories, "Id", "Name", articleContent.CategoryId);
			return View(articleContent);
		}

		// GET: ArticleContents/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var articleContent = DbContext.ArticleContents.Find(id);
			if (articleContent == null)
			{
				return HttpNotFound();
			}
			return View(articleContent);
		}

		// POST: ArticleContents/Edit/5
		// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
		// 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateInput(false)]
		public ActionResult Edit([Bind(Include = "Id,CategoryId,Title,DisplayOrder,CreateBy,CreateTime,TopicImage,Author,Content")] ArticleContent
				articleContent)
		{
			if (ModelState.IsValid)
			{
				DbContext.Entry(articleContent).State = EntityState.Modified;
				DbContext.SaveChanges();
				return RedirectToAction("Index", "Contents", new {categoryId = articleContent.CategoryId});
			}
			return View(articleContent);
		}

		// GET: ArticleContents/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var articleContent = DbContext.ArticleContents.Find(id);
			if (articleContent == null)
			{
				return HttpNotFound();
			}
			return View(articleContent);
		}

		// POST: ArticleContents/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			var articleContent = DbContext.ArticleContents.Find(id);
			DbContext.Contents.Remove(articleContent);
			DbContext.SaveChanges();
			return RedirectToAction("Index", "Contents", new {categoryId = articleContent.CategoryId});
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
