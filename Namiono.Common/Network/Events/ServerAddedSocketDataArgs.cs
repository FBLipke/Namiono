using System;

namespace Namiono.Common.Network.Sockets
{
	public class ServerAddedSocketArgs : EventArgs
	{
		public Guid Server { get; private set; }

		public Guid Socket { get; private set; }

		public ServerAddedSocketArgs(Guid server, Guid socket)
		{
			Server = server;
			Socket = socket;
		}
	}
}
