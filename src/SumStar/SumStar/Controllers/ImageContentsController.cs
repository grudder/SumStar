using System;
using System.Configuration;
using System.Data.Entity;
using System.IO;
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
	public class ImageContentsController : Controller
	{
		private readonly CategoryService _categoryService;
		private ApplicationDbContext _dbContext;

		private readonly string _uploadPath = ConfigurationManager.AppSettings["UploadPath"];

		public ImageContentsController()
		{
		}

		public ImageContentsController(ApplicationDbContext dbContext)
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

		// GET: ImageContents/Create?categoryId=5
		public ActionResult Create(int categoryId)
		{
			var imageContent = new ImageContent
			{
				CategoryId = categoryId,
				Category = DbContext.Categories.Find(categoryId)
			};

			return View(imageContent);
		}

		// POST: ImageContents/Create?categoryId=5
		// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
		// 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateInput(false)]
		public ActionResult Create([Bind(Include = "Id,CategoryId,Title,DisplayOrder,LinkUrl,ImageUrl")] ImageContent imageContent,
			HttpPostedFileBase imageFile)
		{
			imageContent.CreateBy = HttpContext.User.Identity.GetUserId();
			imageContent.CreateTime = DateTime.Now;

			if (ModelState.IsValid)
			{
				if (imageFile != null && imageFile.ContentLength > 0)
				{
					string userName = HttpContext.User.Identity.Name;
					string folderUrl = Path.Combine(_uploadPath, userName);
					string folderPath = Server.MapPath(folderUrl);
					if (!Directory.Exists(folderPath))
					{
						Directory.CreateDirectory(folderPath);
					}
					string fileExtension = imageFile.FileName.Substring(imageFile.FileName.LastIndexOf('.'));
					string fileName = DateTime.Now.ToString("yyyyMMddHHmmssffff") + fileExtension;
					string filePath = Path.Combine(folderPath, fileName);
					imageFile.SaveAs(filePath);

					string fileUrl = folderUrl + "/" + fileName;
					imageContent.ImageUrl = fileUrl;
				}

				DbContext.Contents.Add(imageContent);
				DbContext.SaveChanges();
				return RedirectToAction("Index", "Contents", new {categoryId = imageContent.CategoryId});
			}

			imageContent.Category = DbContext.Categories.Find(imageContent.CategoryId);
			return View(imageContent);
		}

		// GET: ImageContents/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var imageContent = DbContext.ImageContents.Find(id);
			if (imageContent == null)
			{
				return HttpNotFound();
			}
			return View(imageContent);
		}

		// POST: ImageContents/Edit/5
		// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
		// 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateInput(false)]
		public ActionResult Edit([Bind(Include = "Id,CategoryId,Title,DisplayOrder,LinkUrl,ImageUrl,CreateBy,CreateTime")] ImageContent
				imageContent, HttpPostedFileBase imageFile)
		{
			if (ModelState.IsValid)
			{
				if (imageFile != null && imageFile.ContentLength > 0)
				{
					// 删除原有文件
					string oldFilePath = Server.MapPath(imageContent.ImageUrl);
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
					string fileExtension = imageFile.FileName.Substring(imageFile.FileName.LastIndexOf('.'));
					string fileName = DateTime.Now.ToString("yyyyMMddHHmmssffff") + fileExtension;
					string filePath = Path.Combine(folderPath, fileName);
					imageFile.SaveAs(filePath);

					string fileUrl = folderUrl + "/" + fileName;
					imageContent.ImageUrl = fileUrl;
				}

				DbContext.Entry(imageContent).State = EntityState.Modified;
				DbContext.SaveChanges();
				return RedirectToAction("Index", "Contents", new {categoryId = imageContent.CategoryId});
			}
			return View(imageContent);
		}

		// GET: ImageContents/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var imageContent = DbContext.ImageContents.Find(id);
			if (imageContent == null)
			{
				return HttpNotFound();
			}
			return View(imageContent);
		}

		// POST: ImageContents/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			var imageContent = DbContext.ImageContents.Find(id);

			// 删除附件文件
			if (!String.IsNullOrEmpty(imageContent.ImageUrl))
			{
				string oldFilePath = Server.MapPath(imageContent.ImageUrl);
				if (System.IO.File.Exists(oldFilePath))
				{
					System.IO.File.Delete(oldFilePath);
				}
			}

			DbContext.Contents.Remove(imageContent);
			DbContext.SaveChanges();
			return RedirectToAction("Index", "Contents", new {categoryId = imageContent.CategoryId});
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
