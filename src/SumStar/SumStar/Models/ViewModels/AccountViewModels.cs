using System.ComponentModel.DataAnnotations;

namespace SumStar.Models.ViewModels
{
	public class LoginViewModel
	{
		[Required]
		[StringLength(20)]
		[Display(Name = "用户名")]
		public string UserName
		{
			get;
			set;
		}

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "密码")]
		public string Password
		{
			get;
			set;
		}

		[Display(Name = "记住我?")]
		public bool RememberMe
		{
			get;
			set;
		}
	}

	public class RegisterViewModel
	{
		public string Id
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "角色")]
		public string RoleName
		{
			get;
			set;
		}

		[Required]
		[StringLength(20)]
		[Display(Name = "用户名")]
		public string UserName
		{
			get;
			set;
		}

		[Required]
		[StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "密码")]
		public string Password
		{
			get;
			set;
		}

		[DataType(DataType.Password)]
		[Display(Name = "确认密码")]
		[Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
		public string ConfirmPassword
		{
			get;
			set;
		}

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
	}

	public class EditUserViewModel
	{
		public string Id
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "角色")]
		public string RoleName
		{
			get;
			set;
		}

		[Required]
		[StringLength(20)]
		[Display(Name = "用户名")]
		public string UserName
		{
			get;
			set;
		}

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
	}

	public class ResetPasswordViewModel
	{
		public string Id
		{
			get;
			set;
		}

		public string Token
		{
			get;
			set;
		}

		[Required]
		[StringLength(20)]
		[Display(Name = "用户名")]
		public string UserName
		{
			get;
			set;
		}

		[Required]
		[StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "密码")]
		public string Password
		{
			get;
			set;
		}

		[DataType(DataType.Password)]
		[Display(Name = "确认密码")]
		[Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
		public string ConfirmPassword
		{
			get;
			set;
		}
	}
}
