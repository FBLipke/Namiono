using System;

namespace Namiono.Common.Network.Sockets
{
	public class SocketClosedClientEventArgs : EventArgs
	{
		public SocketClosedClientEventArgs(Guid client, Guid socket)
		{
			Client = client;
			Socket = socket;
		}

		public Guid Client { get; private set; }

		public Guid Socket { get; private set; }
	}
}
