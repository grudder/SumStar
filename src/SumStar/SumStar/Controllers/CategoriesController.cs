using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity.Owin;

using SumStar.DataAccess;
using SumStar.Models;

namespace SumStar.Controllers
{
	public class CategoriesController : Controller
	{
		private ApplicationDbContext _dbContext;

		public CategoriesController()
		{
		}

		public CategoriesController(ApplicationDbContext dbContext)
		{
			DbContext = dbContext;
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

		// GET: Categories
		public ActionResult Index()
		{
			var categories = DbContext.Categories.Include(c => c.CreateByUser).Include(c => c.Parent);
			return View(categories.ToList());
		}

		// GET: Categories/Details/5
		public ActionResult Details(int? id)
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

		// GET: Categories/Create
		public ActionResult Create()
		{
			ViewBag.CreateBy = new SelectList(DbContext.Users, "Id", "Remark");
			ViewBag.ParentId = new SelectList(DbContext.Categories, "Id", "Name");
			return View();
		}

		// POST: Categories/Create
		// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
		// 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(
			[Bind(Include = "Id,ParentId,DisplayOrder,Name,ContentType,Remark,CreateBy,CreateTime")] Category category)
		{
			if (ModelState.IsValid)
			{
				DbContext.Categories.Add(category);
				DbContext.SaveChanges();
				return RedirectToAction("Index");
			}

			ViewBag.CreateBy = new SelectList(DbContext.Users, "Id", "Remark", category.CreateBy);
			ViewBag.ParentId = new SelectList(DbContext.Categories, "Id", "Name", category.ParentId);
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
			[Bind(Include = "Id,ParentId,DisplayOrder,Name,ContentType,Remark,CreateBy,CreateTime")] Category category)
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