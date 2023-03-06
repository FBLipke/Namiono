using System;

namespace Namiono.Common.Network.Sockets
{
	public class ClientErrorEventArgs : EventArgs
	{
		public Guid Client { get; set; }

		public Exception Exception { get; set; }

		public ClientErrorEventArgs(Guid client, Exception exception)
		{
			Exception = exception;
			Client = client;
		}
	}
}
