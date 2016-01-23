using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity.Owin;

using SumStar.DataAccess;
using SumStar.Models;
using SumStar.Services;

namespace SumStar.Controllers
{
	public class HomeController : Controller
	{
		private ApplicationDbContext _dbContext;
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

		public ContentService ContentService => _contentService ?? new ContentService(DbContext);

		public HomeController()
		{
		}

		public HomeController(ApplicationDbContext dbContext)
		{
			DbContext = dbContext;
			_contentService = new ContentService(DbContext);
		}

		// GET: /Home
		public ActionResult Index()
		{
			IList<Content> contents = ContentService.GetByCategory("首页滚动图");

			ViewBag.IsEnglish = false;
			return View(contents);
		}

		// GET: /Home/IndexEn
		public ActionResult IndexEn()
		{
			IList<Content> contents = ContentService.GetByCategory("英文首页滚动图");

			ViewBag.IsEnglish = true;
			return View(contents);
		}
	}
}
