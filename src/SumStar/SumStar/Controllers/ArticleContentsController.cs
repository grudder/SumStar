using System;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using PagedList;

using SumStar.DataAccess;
using SumStar.Models;
using SumStar.Services;

namespace SumStar.Controllers
{
	public class ArticleContentsController : Controller
	{
		private ApplicationDbContext _dbContext;
		private readonly CategoryService _categoryService;
		private readonly ContentService _contentService;

		private readonly string _uploadPath = ConfigurationManager.AppSettings["UploadPath"];

		public ArticleContentsController()
		{
		}

		public ArticleContentsController(ApplicationDbContext dbContext)
		{
			DbContext = dbContext;
			_categoryService = new CategoryService(DbContext);
			_contentService = new ContentService(DbContext);
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

		public ContentService ContentService => _contentService ?? new ContentService(DbContext);

		// GET: ArticleContents/List?categoryId=5&pageSize=15&page=1
		public ActionResult List(int categoryId, int? pageSize, int? page)
		{
			IPagedList<ArticleContent> pagedList = ContentService.GetArticleContentsByCategory(categoryId, pageSize, page);

			Category category = DbContext.Categories.Find(categoryId);
			ViewBag.Category = category;
			ViewBag.Level1Category = CategoryService.GetLevel1Category(category);

			return View(pagedList);
		}

		// GET: ArticleContents/Detail/5
		public ActionResult Detail(int? id)
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
			ViewBag.Category = articleContent.Category;
			ViewBag.Level1Category = CategoryService.GetLevel1Category(articleContent.Category);
			return View(articleContent);
		}

		// GET: ArticleContents/Create?categoryId=5
		public ActionResult Create(int categoryId)
		{
			var articleContent = new ArticleContent
			{
				CategoryId = categoryId,
				Category = DbContext.Categories.Find(categoryId),
				TitleVisibleInContent = true
			};
			ViewBag.CategoryId = categoryId;

			return View(articleContent);
		}

		// POST: ArticleContents/Create?categoryId=5
		// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
		// 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateInput(false)]
		public ActionResult Create([Bind(Include = "Id,CategoryId,Title,DisplayOrder,TopicImage,Author,Content")] ArticleContent articleContent,
			HttpPostedFileBase topicImageFile)
		{
			if (topicImageFile != null && topicImageFile.ContentLength > 0)
			{
				string userName = HttpContext.User.Identity.Name;
				string folderUrl = Path.Combine(_uploadPath, userName);
				string folderPath = Server.MapPath(folderUrl);
				if (!Directory.Exists(folderPath))
				{
					Directory.CreateDirectory(folderPath);
				}
				string fileExtension = topicImageFile.FileName.Substring(topicImageFile.FileName.LastIndexOf('.'));
				string fileName = DateTime.Now.ToString("yyyyMMddHHmmssffff") + fileExtension;
				string filePath = Path.Combine(folderPath, fileName);
				topicImageFile.SaveAs(filePath);

				string fileUrl = folderUrl + "/" + fileName;
				articleContent.TopicImage = fileUrl;
			}

			articleContent.CreateBy = HttpContext.User.Identity.GetUserId();
			articleContent.CreateTime = DateTime.Now;

			if (ModelState.IsValid)
			{
				DbContext.Contents.Add(articleContent);
				DbContext.SaveChanges();
				return RedirectToAction("Index", "Contents", new {categoryId = articleContent.CategoryId});
			}

			articleContent.Category = DbContext.Categories.Find(articleContent.CategoryId);
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
			ViewBag.CategoryId = articleContent.CategoryId;
			return View(articleContent);
		}

		// POST: ArticleContents/Edit/5
		// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
		// 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateInput(false)]
		public ActionResult Edit([Bind(Include = "Id,CategoryId,Title,DisplayOrder,TopicImage,Author,Content,CreateBy,CreateTime")] ArticleContent
				articleContent, HttpPostedFileBase topicImageFile)
		{
			if (ModelState.IsValid)
			{
				if (topicImageFile != null && topicImageFile.ContentLength > 0)
				{
					// 删除原有文件
					string oldFilePath = Server.MapPath(articleContent.TopicImage);
					if (System.IO.File.Exists(oldFilePath))
					{
						System.IO.File.Delete(oldFilePath);
					}

					// 保存新上传的文件
					string userName = HttpContext.User.Identity.Name;
					string folderUrl = Path.Combine(_uploadPath, userName);
					string folderPath = Server.MapPath(folderUrl);
					if (!Directory.Exists(folderPath))
					{
						Directory.CreateDirectory(folderPath);
					}
					string fileExtension = topicImageFile.FileName.Substring(topicImageFile.FileName.LastIndexOf('.'));
					string fileName = DateTime.Now.ToString("yyyyMMddHHmmssffff") + fileExtension;
					string filePath = Path.Combine(folderPath, fileName);
					topicImageFile.SaveAs(filePath);

					string fileUrl = folderUrl + "/" + fileName;
					articleContent.TopicImage = fileUrl;
				}

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
			ViewBag.CategoryId = articleContent.CategoryId;
			return View(articleContent);
		}

		// POST: ArticleContents/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			var articleContent = DbContext.ArticleContents.Find(id);

			// 删除附件文件
			if (!String.IsNullOrEmpty(articleContent.TopicImage))
			{
				string oldFilePath = Server.MapPath(articleContent.TopicImage);
				if (System.IO.File.Exists(oldFilePath))
				{
					System.IO.File.Delete(oldFilePath);
				}
			}

			DbContext.ArticleContents.Remove(articleContent);
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
