using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SumStar.Models
{
	[Table("T_OperationLog")]
	[DisplayName("操作日志")]
	public class OperationLog
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "页面地址")]
		[StringLength(200)]
		public string PageUrl
		{
			get;
			set;
		}

		[Display(Name = "描述")]
		[StringLength(200)]
		public string Description
		{
			get;
			set;
		}

		/// <summary>
		/// 操作人的标识。
		/// </summary>
		public string CreateBy
		{
			get;
			set;
		}

		[Display(Name = "操作人")]
		[Required]
		[ForeignKey("CreateBy")]
		public virtual ApplicationUser CreateByUser
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "操作时间")]
		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
		public DateTime CreateTime
		{
			get;
			set;
		}
	}
}