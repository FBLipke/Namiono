using System;
using System.Linq;
using System.Text;

namespace Namiono.Common
{
	public static class Extensions
	{
		public static string Captitalize(this string str) => str.First().ToString().ToUpper() + str.Substring(1);

		public static string GetString_UTF8(this byte[] input) => Encoding.UTF8.GetString(input);

		public static string As_UTF8_String(this string str) => Encoding.ASCII.GetBytes(str).GetString_UTF8();

		public static bool ContainsChar(this string str, char[] patterns)
		{
			bool flag = false;
			foreach (char pattern in patterns)
			{
				if (str.Contains(pattern))
					flag = true;
			}
			return flag;
		}

		public static string FromBase64(this string input) => Encoding.UTF8.GetString(Convert.FromBase64String(input));

		public static double AsUnixTimeStamp(this DateTime dt) => DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

		public static string Append(this string str, string seperator, string appending) => string.Join(seperator, new string[2]
		{
	  str,
	  appending
		});

		public static DateTime FirstDayOfWeek()
		{
			var dayOfWeek = (int)DateTime.Now.DayOfWeek;
			return DateTime.Today.AddDays(dayOfWeek == 0 ? -6.0 : -dayOfWeek + 1);
		}

		public static string AsString<T>(this T input) => string.Format("{0}", input);

		public static byte[] GetBytes_UTF8(this string str) => string.IsNullOrEmpty(str) ? new byte[0] : Encoding.UTF8.GetBytes(str);
	}
}
