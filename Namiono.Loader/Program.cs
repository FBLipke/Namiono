// Decompiled with JetBrains decompiler
// Type: Namiono.Loader.Program
// Assembly: Namiono.Loader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8F840A-6490-428A-B4BD-EF625A5083D4
// Assembly location: C:\Users\LipkeGu\Desktop\namiono___\Namiono.Loader.exe

using Namiono.Common;
using System;

namespace Namiono.Loader
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			using (NamionoCommon namionoCommon = new NamionoCommon())
			{
				namionoCommon.Bootstrap();
				namionoCommon.Install();
				namionoCommon.Start();
				string str = string.Empty;
				while (str != "!exit")
					str = Console.ReadLine();
				namionoCommon.Stop();
				namionoCommon.Close();
			}
		}
	}
}
