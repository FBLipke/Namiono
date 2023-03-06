using System;

namespace Namiono.Common.Network.Sockets
{
	public class ClientConnectionClosedEventArgs : EventArgs
	{
		public Guid Client { get; private set; }

		public ClientConnectionClosedEventArgs(Guid client) => Client = client;
	}
}
