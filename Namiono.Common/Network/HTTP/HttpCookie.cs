using System;
using System.Globalization;

namespace Namiono.Common.Network.HTTP
{
	public class HttpCookie
	{
		public string Name { get; private set; }

		public string Value { get; private set; }

		public string Expire { get; private set; }

		public bool Remove { get; set; }

		public bool Expired { get; private set; }

		public HttpCookie(string name, string value)
		{
			Name = name;
			Value = value;
			var dateTime = DateTime.Now;
			dateTime = dateTime.AddDays(1.0);
			Expire = dateTime.ToString("ddd, MMM dd yyyy HH:mm:ss 'GMT'K", CultureInfo.InvariantCulture);
		}
	}
}
