using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SumStar.Models
{
	// 可以通过向 ApplicationUser 类添加更多属性来为用户添加配置文件数据。若要了解详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=317594。
	public class ApplicationUser : IdentityUser
	{
		#region 自定义属性

		/// <summary>
		/// 备注
		/// </summary>
		[Display(Name = "备注")]
		[StringLength(200)]
		[DataType(DataType.MultilineText)]
		public string Remark
		{
			get;
			set;
		}

		#endregion

		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
		{
			// 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
			ClaimsIdentity userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			// 在此处添加自定义用户声明
			return userIdentity;
		}
	}

	/// <summary>
	/// 可以通过向 ApplicationRole 类添加更多属性来为角色添加配置文件数据。
	/// </summary>
	public class ApplicationRole : IdentityRole
	{
		#region 自定义属性

		/// <summary>
		/// 备注
		/// </summary>
		[Display(Name = "备注")]
		[StringLength(200)]
		[DataType(DataType.MultilineText)]
		public string Remark
		{
			get;
			set;
		}

		#endregion
	}
}