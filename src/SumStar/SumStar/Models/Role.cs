using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SumStar.Models
{
	[Table("T_Role")]
	[DisplayName("角色")]
	[DisplayColumn("Name")]
	public class Role
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id
		{
			get;
			set;
		}

		[Display(Name = "名称")]
		[Required]
		[StringLength(20)]
		public string Name
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