using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SumStar.Models
{
	[DisplayName("文章")]
	[Table("T_Article")]
	[DisplayColumn("Title")]
	public class Article
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id
		{
			get;
			set;
		}

		/// <summary>
		/// 所属栏目的标识
		/// </summary>
		public int? CategoryId
		{
			get;
			set;
		}

		[Display(Name = "所属栏目")]
		[Required]
		[ForeignKey("CategoryId")]
		public virtual Category Category
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

		[Display(Name = "标题")]
		[Required]
		[StringLength(100)]
		public string Title
		{
			get;
			set;
		}

		[Display(Name = "题图")]
		[StringLength(200)]
		public string TopicImage
		{
			get;
			set;
		}

		[Display(Name = "作者")]
		[StringLength(20)]
		public string Author
		{
			get;
			set;
		}

		[Display(Name = "内容")]
		[Column(TypeName = "text")]
		[DataType(DataType.MultilineText)]
		public string Content
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

		[Display(Name = "创建时间")]
		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = false)]
		public DateTime CreateTime
		{
			get;
			set;
		}
	}
}