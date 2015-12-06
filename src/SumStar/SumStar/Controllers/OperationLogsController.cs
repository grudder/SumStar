using System.Linq;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity.Owin;

using PagedList;

using SumStar.DataAccess;
using SumStar.Models;

namespace SumStar.Controllers
{
	public class OperationLogsController : Controller
	{
		private ApplicationDbContext _dbContext;

		public OperationLogsController()
		{
		}

		public OperationLogsController(ApplicationDbContext dbContext)
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

		// GET: OperationLogs
		public ActionResult Index(int? pageSize, int? page)
		{
			var query = from log in DbContext.OperationLogs
				orderby log.CreateTime descending
				select log;
			// 分页处理
			pageSize = (pageSize ?? 20);
			page = (page ?? 1);
			IPagedList<OperationLog> pagedList = query.ToPagedList(page.Value, pageSize.Value);

			return View(pagedList);
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
