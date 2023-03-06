using System;

namespace Namiono.Common.Network.Sockets
{
	public class ServerClosedClientConnectionArgs : EventArgs
	{
		public Guid Server { get; private set; }

		public Guid Socket { get; private set; }

		public Guid Client { get; private set; }

		public ServerClosedClientConnectionArgs(Guid server, Guid socket, Guid client)
		{
			Server = server;
			Socket = socket;
			Client = client;
		}
	}
}
