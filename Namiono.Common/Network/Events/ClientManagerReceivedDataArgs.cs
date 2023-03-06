using System;

namespace Namiono.Common.Network
{
	public class ClientManagerReceivedDataArgs : EventArgs
	{
		public Guid Client { get; private set; }

		public byte[] Data { get; private set; }

		public ClientManagerReceivedDataArgs(Guid client, byte[] data)
		{
			Client = client;
			Data = data;
		}
	}
}
