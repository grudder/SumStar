using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;

using SumStar.Helper;

namespace SumStar.Models
{
	/// <summary>
	/// 采用【TPT】方式实现继承关系
	/// </summary>
	[DisplayName("内容")]
	[Table("T_Content")]
	[DisplayColumn("Title")]
	public class Content
	{
		[Key]
		public int Id
		{
			get;
			set;
		}

		/// <summary>
		/// 所属栏目的标识
		/// </summary>
		public int CategoryId
		{
			get;
			set;
		}
		
		[Display(Name = "所属栏目")]
		[ForeignKey("CategoryId")]
		public virtual Category Category
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "标题")]
		[StringLength(100)]
		public string Title
		{
			get;
			set;
		}

		[Display(Name = "显示顺序")]
		public int DisplayOrder
		{
			get;
			set;
		}

		/// <summary>
		/// 创建人的标识。
		/// </summary>
		public string CreateBy
		{
			get;
			set;
		}

		[Display(Name = "创建人")]
		[ForeignKey("CreateBy")]
		public virtual ApplicationUser CreateByUser
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "创建时间")]
		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = false)]
		[JsonConverter(typeof(MyDateTimeConverter), "yyyy-MM-dd HH:mm:ss")]
		public DateTime CreateTime
		{
			get;
			set;
		}
	}
}