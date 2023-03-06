// Decompiled with JetBrains decompiler
// Type: Namiono.Module.IsRuntimeModuleAttribute
// Assembly: Namiono.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CE4FCADF-C52D-4962-B4B8-C6D36FAB8FAE
// Assembly location: C:\Users\LipkeGu\Desktop\namiono___\Namiono.Common.dll

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
