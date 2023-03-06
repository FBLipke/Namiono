using System;

namespace Namiono.Common.Network.Sockets
{
	public class SocketReadDataFromClientArgs : EventArgs
	{
		public byte[] Data { get; private set; }

		public Guid Client { get; private set; }

		public Guid Socket { get; private set; }

		public SocketReadDataFromClientArgs(Guid socket, Guid client, byte[] data)
		{
			Socket = socket;
			Data = data;
			Client = client;
		}
	}
}
