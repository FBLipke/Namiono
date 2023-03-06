using System;

namespace Namiono.Common.Network.Sockets
{
	public class ClientConnectedEventArgs : EventArgs
	{
		public Guid Client { get; private set; }

		public ClientConnectedEventArgs(Guid client) => Client = client;
	}
}
