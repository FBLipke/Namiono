using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Namiono.Common.Network
{
	public class HttpProcessor : IDisposable
	{
		public static HttpRequest GetRequest(MemoryStream data, byte[] rawBytes)
		{
			var req = new HttpRequest();
			using (var streamReader1 = new StreamReader(data))
			{
				var str1 = streamReader1.ReadLine();
				if (str1 != null)
				{
					var source = str1.Split(' ');
					req.Method = ((IEnumerable<string>)source).First().ToUpper();
					if (source.Length < 2)
						return null;
					if (source[1].Contains("?"))
					{
						req.Path = ((IEnumerable<string>)source[1].Split('?')).First();
						ParseParameters(ref req, HttpUtility.UrlDecode(source[1].Split('?')[1]));
					}
					else
						req.Path = source[1];
					req.Version = source[2];
				}
				while (!streamReader1.EndOfStream)
				{
					var str2 = streamReader1.ReadLine()?.Trim();
					if (!string.IsNullOrEmpty(str2))
					{
						var length = str2.IndexOf(':');
						if (length != -1)
						{
							var key = str2.Substring(0, length);
							var num = length + 1;
							while (num < str2.Length && str2[num] == ' ')
								++num;
							if (!req.Headers.ContainsKey(key))
								req.Headers.Add(key, str2.Substring(num, str2.Length - num));
							var numArray1 = new byte[0];
							if (req.Headers.ContainsKey("Cookie"))
							{
								req.Cookies.AddRange(((IEnumerable<string>)req.Headers["Cookie"].Split(';')).Select(p => p.Trim()).Select(cookie => new
								{
									cookie,
									kv = cookie.Split('=')
								}).Select(_param1 => new HTTP.HttpCookie(_param1.kv[0], _param1.kv[1])));
								req.Headers.Remove("Cookie");
							}
							if (req.Headers.ContainsKey("Content-Length"))
							{
								var int32 = Convert.ToInt32(req.Headers["Content-Length"]);
								var numArray2 = new byte[int32];
								Array.Copy(rawBytes, rawBytes.Length - int32, numArray2, 0, int32);
								using (var memoryStream = new MemoryStream(numArray2))
								{
									using (var streamReader2 = new StreamReader(memoryStream))
									{
										string str3 = streamReader2.ReadLine().As_UTF8_String();
										ParseParameters(ref req, str3);
										streamReader2.Close();
									}
								}
							}
						}
					}
					else
						break;
				}
				HTTP.HttpCookie httpCookie = req.Cookies.Where(c => c.Name == "UserId").FirstOrDefault();
				if (httpCookie != null && NamionoCommon.Providers.ContainsKey("User") && !string.IsNullOrEmpty(httpCookie.Value))
				{
					req.User = NamionoCommon.Providers["User"].Request(Guid.Parse(httpCookie.Value));
					if (req.User != null)
						req.Headers["User"] = req.User.Id.ToString();
				}
			}
			return req;
		}

		private static void ParseParameters(ref HttpRequest req, string param)
		{
			if (param == null || !param.Contains("?") && !param.Contains("&"))
				return;
			string str1 = param;
			char[] chArray = new char[1] { '&' };
			foreach (string str2 in str1.Split(chArray))
			{
				if (str2.Contains("="))
				{
					string[] strArray = str2.Split('=');
					if (!req.Parameters.ContainsKey(strArray[0]))
					{
						if (strArray[0] == "_")
							strArray[0] = "Timestamp";
						if (!string.IsNullOrEmpty(strArray[1]) && !string.IsNullOrWhiteSpace(strArray[1]))
							req.Parameters.Add(strArray[0], HttpUtility.UrlDecode(strArray[1]));
					}
				}
			}
		}

		public void Dispose()
		{
		}
	}
}
