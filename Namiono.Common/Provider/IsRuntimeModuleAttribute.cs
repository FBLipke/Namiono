using Namiono.Common;
using System;

namespace Namiono.Module
{
	public class IsRuntimeModuleAttribute : Attribute
	{
		public IsRuntimeModuleAttribute()
			=> NamionoCommon.Log("I", "Namiono.ModuleInstaller",
				"This Module does not need any Installation Routines!");
	}
}
