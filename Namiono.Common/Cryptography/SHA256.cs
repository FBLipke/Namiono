// Decompiled with JetBrains decompiler
// Type: Namiono.Common.SHA256
// Assembly: Namiono.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CE4FCADF-C52D-4962-B4B8-C6D36FAB8FAE
// Assembly location: C:\Users\LipkeGu\Desktop\namiono___\Namiono.Common.dll

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
