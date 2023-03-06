using System;

namespace Namiono.Common.Network
{
	public class NetworkManagerRequestHandledEventArgs : EventArgs
	{
		public Guid Server { get; private set; }

		public Guid Socket { get; private set; }

		public Guid Client { get; private set; }

		public bool KeepAlive { get; private set; }

		public HttpResponse Response { get; private set; }

		public NetworkManagerRequestHandledEventArgs(
		  Guid server,
		  Guid socket,
		  Guid client,
		  HttpResponse response)
		{
			Server = server;
			Socket = socket;
			Client = client;
			Response = response;
		}
	}
}
