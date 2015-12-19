using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SumStar.Models
{
	[DisplayName("图片内容")]
	[Table("T_ImageContent")]
	[DisplayColumn("Title")]
	public class ImageContent : Content
	{
		[Required]
		[Display(Name = "链接地址")]
		[StringLength(200)]
		public string LinkUrl
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "图片地址")]
		[StringLength(200)]
		public string ImageUrl
		{
			get;
			set;
		}
	}
}