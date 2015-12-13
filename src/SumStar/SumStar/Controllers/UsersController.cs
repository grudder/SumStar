using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using SumStar.Models;
using SumStar.Models.ViewModels;

namespace SumStar.Controllers
{
	[Authorize(Roles = "SysAdmin")]
	public class UsersController : Controller
	{
		private ApplicationUserManager _userManager;

		private ApplicationRoleManager _roleManager;

		public UsersController()
		{
		}

		public UsersController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
		{
			UserManager = userManager;
			RoleManager = roleManager;
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

		public ApplicationRoleManager RoleManager
		{
			get
			{
				return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
			}
			private set
			{
				_roleManager = value;
			}
		}

		// GET: Users
		public ActionResult Index()
		{
			return View(UserManager.Users);
		}

		// GET: Users/Create
		public ActionResult Create()
		{
			var listItems = RoleManager.Roles.Select(role => new SelectListItem
			{
				Value = role.Name,
				Text = role.Remark
			}).ToList();
			ViewBag.Roles = listItems;

			return View();
		}

		//
		// POST: /Users/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				ApplicationUser user = new ApplicationUser
				{
					UserName = model.UserName,
					Remark = model.Remark
				};
				var result = await UserManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					result = await UserManager.AddToRoleAsync(user.Id, model.RoleName);
					if (result.Succeeded)
					{
						return RedirectToAction("Index");
					}
				}
				AddErrors(result);
			}

			var listItems = RoleManager.Roles.Select(role => new SelectListItem
			{
				Value = role.Name,
				Text = role.Remark
			}).ToList();
			ViewBag.Roles = listItems;

			// 如果我们进行到这一步时某个地方出错，则重新显示表单
			return View(model);
		}

		// GET: Users/Edit/5
		public async Task<ActionResult> Edit(string id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			ApplicationUser user = await UserManager.FindByIdAsync(id);
			if (user == null)
			{
				return HttpNotFound();
			}

			IList<string> userRoles = await UserManager.GetRolesAsync(id);
			var viewModel = new EditUserViewModel
			{
				Id = user.Id,
				RoleName = userRoles.Single(),
				UserName = user.UserName,
				Remark = user.Remark
			};
			
			var listItems = RoleManager.Roles.Select(role => new SelectListItem
			{
				Value = role.Name,
				Text = role.Remark,
				Selected = (role.Name == viewModel.RoleName)
			}).ToList();
			ViewBag.Roles = listItems;

			return View(viewModel);
		}

		// POST: Users/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit(EditUserViewModel model)
		{
			if (ModelState.IsValid)
			{
				ApplicationUser user = await UserManager.FindByIdAsync(model.Id);
				user.Remark = model.Remark;
				IdentityResult result = await UserManager.UpdateAsync(user);
				if (result.Succeeded)
				{
					IList<string> userRoles = await UserManager.GetRolesAsync(model.Id);
					result = await UserManager.RemoveFromRoleAsync(user.Id, userRoles.Single());
					if (result.Succeeded)
					{
						result = await UserManager.AddToRoleAsync(user.Id, model.RoleName);
						if (result.Succeeded)
						{
							return RedirectToAction("Index");
						}
					}
				}
				AddErrors(result);
			}

			var listItems = RoleManager.Roles.Select(role => new SelectListItem
			{
				Value = role.Name,
				Text = role.Remark,
				Selected = (role.Name == model.RoleName)
			}).ToList();
			ViewBag.Roles = listItems;

			// 如果我们进行到这一步时某个地方出错，则重新显示表单
			return View(model);
		}

		//
		// GET: /Users/ResetPassword
		public async Task<ActionResult> ResetPassword(string id)
		{
			ApplicationUser user = await UserManager.FindByIdAsync(id);
			if (user == null)
			{
				return HttpNotFound();
			}

			string token = await UserManager.GeneratePasswordResetTokenAsync(id);
			var viewModel = new ResetPasswordViewModel
			{
				Id = user.Id,
				Token = token,
				UserName = user.UserName
			};

			return View(viewModel);
		}

		//
		// POST: /Users/ResetPassword
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var user = await UserManager.FindByIdAsync(model.Id);
			if (user == null)
			{
				return HttpNotFound();
			}

			var result = await UserManager.ResetPasswordAsync(user.Id, model.Token, model.Password);
			if (result.Succeeded)
			{
				return RedirectToAction("Index");
			}
			AddErrors(result);
			return View();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_userManager != null)
				{
					_userManager.Dispose();
					_userManager = null;
				}

				if (_roleManager != null)
				{
					_roleManager.Dispose();
					_roleManager = null;
				}
			}

			base.Dispose(disposing);
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}
	}
}
