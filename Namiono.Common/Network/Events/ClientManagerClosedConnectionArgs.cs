using System;

namespace Namiono.Common.Network
{
	public class ClientManagerClosedConnectionArgs : EventArgs
	{
		public Guid Client { get; private set; }

		public ClientManagerClosedConnectionArgs(Guid client) => Client = client;
	}
}
