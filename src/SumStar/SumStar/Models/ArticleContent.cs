using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SumStar.Models
{
	[DisplayName("文章内容")]
	[Table("T_ArticleContent")]
	[DisplayColumn("Title")]
	public class ArticleContent : Content
	{
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

		[Required]
		[Display(Name = "内容")]
		[Column(TypeName = "text")]
		[DataType(DataType.MultilineText)]
		public string Content
		{
			get;
			set;
		}
	}
}