using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SumStar.Models
{
	[Table("T_User")]
	[DisplayName("用户")]
	[DisplayColumn("Name")]
	public class User
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id
		{
			get;
			set;
		}

		[Display(Name = "用户名")]
		[Required]
		[StringLength(20)]
		public string Name
		{
			get;
			set;
		}

		[Display(Name = "密码")]
		[Required]
		[StringLength(40)]
		[DataType(DataType.Password)]
		public string Password
		{
			get;
			set;
		}

		[Display(Name = "姓名")]
		[Required]
		[StringLength(10)]
		public string FullName
		{
			get;
			set;
		}

		[Display(Name = "备注")]
		[StringLength(200)]
		[DataType(DataType.MultilineText)]
		public string Remark
		{
			get;
			set;
		}

		[Display(Name = "创建人")]
		public int CreateBy
		{
			get;
			set;
		}

		[Display(Name = "创建时间")]
		[DataType(DataType.DateTime)]
		public DateTime CreateTime
		{
			get;
			set;
		}
	}
}