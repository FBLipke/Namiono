using Namiono.Common;
using System;

namespace Namiono.Loader
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			using (var namionoCommon = new NamionoCommon())
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
