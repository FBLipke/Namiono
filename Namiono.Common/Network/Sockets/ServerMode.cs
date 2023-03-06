using Namiono.Network.Sockets;

namespace Namiono.Common.Network.Sockets
{
	public enum ServerMode
	{
		Http,
		HttpMedia,
		DHCP,
		TFTP,
		BOOTP

	}

	public delegate void ServerAddedSocketEventHandler(INamionoServer sender, ServerAddedSocketArgs e);

	public delegate void ServerClosedSocketEventHandler(
	  INamionoServer sender,
	  ServerClosedSocketArgs e);

	public delegate void ServerClosedClientConnectionEventHandler(
	  INamionoServer sender,
	  ServerClosedClientConnectionArgs e);


	public delegate void ServerReceivedDataEventHandler(INamionoServer sender, ServerReceivedDataArgs e);

}
