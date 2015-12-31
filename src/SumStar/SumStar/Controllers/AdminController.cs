using System.Web.Mvc;

namespace SumStar.Controllers
{
	[Authorize]
	public class AdminController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}
	}
}