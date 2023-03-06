// Decompiled with JetBrains decompiler
// Type: Namiono.Common.IService
// Assembly: Namiono.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CE4FCADF-C52D-4962-B4B8-C6D36FAB8FAE
// Assembly location: C:\Users\LipkeGu\Desktop\namiono___\Namiono.Common.dll

namespace Namiono.Common
{
	public interface IService : IManager
	{
		bool Enabled { get; set; }

		bool IsLinux { get; set; }

		string Name { get; set; }

		string Arguments { get; set; }

		string FileName { get; set; }

		string WorkingDir { get; set; }

		bool IsRunning { get; set; }
	}
}
