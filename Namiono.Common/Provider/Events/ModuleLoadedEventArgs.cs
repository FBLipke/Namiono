// Decompiled with JetBrains decompiler
// Type: Namiono.Common.Provider.ModuleLoadedEventArgs
// Assembly: Namiono.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CE4FCADF-C52D-4962-B4B8-C6D36FAB8FAE
// Assembly location: C:\Users\LipkeGu\Desktop\namiono___\Namiono.Common.dll

namespace Namiono.Common.Provider
{
	public class ModuleLoadedEventArgs
	{
		public IProvider Module { get; private set; }

		public string Name { get; private set; }

		public ModuleLoadedEventArgs(string name, IProvider module)
		{
			Name = name.Substring(name.LastIndexOf('.') + 1);
			Module = module;
		}
	}
}
