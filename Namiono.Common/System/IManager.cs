// Decompiled with JetBrains decompiler
// Type: Namiono.Common.IManager
// Assembly: Namiono.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CE4FCADF-C52D-4962-B4B8-C6D36FAB8FAE
// Assembly location: C:\Users\LipkeGu\Desktop\namiono___\Namiono.Common.dll

namespace Namiono.Common
{
	public interface IManager
	{
		void Start();

		void Stop();

		void HeartBeat();

		void Bootstrap();

		void Close();

		void Dispose();
	}
}
