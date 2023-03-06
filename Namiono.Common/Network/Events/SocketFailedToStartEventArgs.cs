using System;
using System.Net.Sockets;

namespace Namiono.Common.Network.Sockets
{
	public class SocketFailedToStartEventArgs : EventArgs
	{
		public Guid Socket { get; private set; }

		public SocketException Exception { get; private set; }

		public SocketFailedToStartEventArgs(Guid socket, SocketException exception)
		{
			Exception = exception;
			Socket = socket;
		}
	}
}
