using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

using SumStar.DataAccess;
using SumStar.Models;
using SumStar.Models.ViewModels;

namespace SumStar.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		private ApplicationDbContext _dbContext;
		private ApplicationUserManager _userManager;
		private ApplicationSignInManager _signInManager;

		public AccountController()
		{
		}

		public AccountController(ApplicationDbContext dbContext, ApplicationUserManager userManager, ApplicationSignInManager signInManager)
		{
			DbContext = dbContext;
			UserManager = userManager;
			SignInManager = signInManager;
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

		public ApplicationUserManager UserManager
		{
			get
			{
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set
			{
				_userManager = value;
			}
		}

		public ApplicationSignInManager SignInManager
		{
			get
			{
				return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			}
			private set
			{
				_signInManager = value;
			}
		}

		//
		// GET: /Account/Login
		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		//
		// POST: /Account/Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// 这不会计入到为执行帐户锁定而统计的登录失败次数中
			// 若要在多次输入错误密码的情况下触发帐户锁定，请更改为 shouldLockout: true
			var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
			switch (result)
			{
				case SignInStatus.Success:
					// 写入登录操作日志
					ApplicationUser user = await UserManager.FindByNameAsync(model.UserName);
					string userId = user.Id;
					var operationLog = new OperationLog
					{
						PageUrl = "/Account/Login",
						Description = "用户登录",
						CreateBy = userId,
						CreateTime = DateTime.Now
					};
					DbContext.OperationLogs.Add(operationLog);
					await DbContext.SaveChangesAsync();
					return RedirectToLocal(returnUrl);
				case SignInStatus.LockedOut:
					return View("Lockout");
				default:
					ModelState.AddModelError("", "无效的登录尝试。");
					return View(model);
			}
		}

		//
		// GET: /Account/Register
		[AllowAnonymous]
		public ActionResult Register()
		{
			return View();
		}

		//
		// POST: /Account/Register
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new ApplicationUser
				{
					UserName = model.UserName,
					Remark = model.Remark
				};
				var result = await UserManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					await SignInManager.SignInAsync(user, false, false);
					
					return RedirectToAction("Index", "Home");
				}
				AddErrors(result);
			}

			// 如果我们进行到这一步时某个地方出错，则重新显示表单
			return View(model);
		}

		//
		// POST: /Account/LogOff
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
			return RedirectToAction("Index", "Home");
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

				if (_userManager != null)
				{
					_userManager.Dispose();
					_userManager = null;
				}

				if (_signInManager != null)
				{
					_signInManager.Dispose();
					_signInManager = null;
				}
			}

			base.Dispose(disposing);
		}

		#region 帮助程序

		// 用于在添加外部登录名时提供 XSRF 保护
		private const string XsrfKey = "XsrfId";

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return RedirectToAction("Index", "Home");
		}

		internal class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string redirectUri)
				: this(provider, redirectUri, null)
			{
			}

			public ChallengeResult(string provider, string redirectUri, string userId)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}

			public string LoginProvider
			{
				get;
				set;
			}

			public string RedirectUri
			{
				get;
				set;
			}

			public string UserId
			{
				get;
				set;
			}

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties
				{
					RedirectUri = RedirectUri
				};
				if (UserId != null)
				{
					properties.Dictionary[XsrfKey] = UserId;
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}

		#endregion
	}
}