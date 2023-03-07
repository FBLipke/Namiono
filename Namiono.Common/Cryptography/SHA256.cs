using System;
using System.Security.Cryptography;
using System.Text;

namespace Namiono.Common
{
	public class SHA256 : ICrypto
	{
		public string GetHash(string text, string key)
		{
			if (string.IsNullOrEmpty(text))
				return string.Empty;

			var buffer = new byte[0];

			using (var hmacshA256 = new HMACSHA256(Encoding.ASCII.GetBytes(key.ToCharArray())))
				buffer = hmacshA256.ComputeHash(buffer);

			return BitConverter.ToString(buffer).Replace("-", string.Empty).ToLower();
		}
	}
}
