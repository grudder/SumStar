﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SumStar.Models
{
	[Table("T_Category")]
	[DisplayName("栏目")]
	public class Category
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id
		{
			get;
			set;
		}

		[Display(Name = "上级栏目")]
		public int? ParentId
		{
			get;
			set;
		}

		[Display(Name = "上级栏目")]
		[ForeignKey("ParentId")]
		public virtual Category Parent
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
		[StringLength(20)]
		public string Name
		{
			get;
			set;
		}

		[Display(Name = "内容类型")]
		public ContentType ContentType
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