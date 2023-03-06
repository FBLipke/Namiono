// Decompiled with JetBrains decompiler
// Type: Namiono.Common.ILogin
// Assembly: Namiono.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CE4FCADF-C52D-4962-B4B8-C6D36FAB8FAE
// Assembly location: C:\Users\LipkeGu\Desktop\namiono___\Namiono.Common.dll

using Namiono.Common.Network;
using Namiono.Common.Provider;

namespace Namiono.Common
{
	public interface ILogin
	{
		IMember Handle_Login_Request(NamionoHttpContext request);

		IMember Handle_Logout_Request(NamionoHttpContext request);
	}
}
