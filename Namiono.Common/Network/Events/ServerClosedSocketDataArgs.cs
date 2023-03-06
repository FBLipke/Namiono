using System;

namespace Namiono.Common.Network.Sockets
{
	public class ServerClosedSocketArgs : EventArgs
	{
		public Guid Server { get; private set; }

		public Guid Socket { get; private set; }

		public ServerClosedSocketArgs(Guid server, Guid socket)
		{
			Server = server;
			Socket = socket;
		}
	}
}
