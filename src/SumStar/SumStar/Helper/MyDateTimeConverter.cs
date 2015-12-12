using Newtonsoft.Json.Converters;

namespace SumStar.Helper
{
	public class MyDateTimeConverter : IsoDateTimeConverter
	{
		public MyDateTimeConverter(string dateTimeFormat)
		{
			DateTimeFormat = dateTimeFormat;
		}
	}
}
