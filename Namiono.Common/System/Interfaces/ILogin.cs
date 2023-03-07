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
