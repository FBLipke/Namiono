using System;
using System.Security.Cryptography;
using System.Text;

namespace Namiono.Common
{
	public class MD5 : ICrypto
	{
		public string GetHash(string text, string key = "notNeededforMD5")
		{
			if (string.IsNullOrEmpty(text))
				return string.Empty;

			var numArray = (byte[])null;

			using (var cryptoServiceProvider = new MD5CryptoServiceProvider())
				numArray = cryptoServiceProvider.ComputeHash(Encoding.ASCII.GetBytes(text));

			return BitConverter.ToString(numArray).Replace("-", string.Empty).ToLower();
		}
	}
}
