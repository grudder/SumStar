﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SumStar.Models
{
	[Table("T_ImageLink")]
	[DisplayName("图片链接")]
	public class ImageLink
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id
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

		[Display(Name = "名称")]
		[Required]
		[StringLength(100)]
		public string Title
		{
			get;
			set;
		}

		[Display(Name = "链接地址")]
		[StringLength(200)]
		public string LinkUrl
		{
			get;
			set;
		}

		[Display(Name = "图片地址")]
		[StringLength(200)]
		public string ImageUrl
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
		[Required]
		[ForeignKey("CreateBy")]
		public virtual ApplicationUser CreateByUser
		{
			get;
			set;
		}

		[Display(Name = "创建时间")]
		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
		public DateTime CreateTime
		{
			get;
			set;
		}
	}
}