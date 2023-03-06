using System;

namespace Namiono.Common.Network
{
	public class HTTPRequestReceivedEventArgs : EventArgs
	{
		public NamionoHttpContext Context { get; private set; }

		public Guid Server { get; private set; }

		public Guid Socket { get; private set; }

		public Guid Client { get; private set; }

		public bool MediaRequest { get; private set; }
		public HTTPRequestReceivedEventArgs(
  Guid server,
  Guid socket,
  Guid client,
  bool icyData,
  NamionoHttpContext context)
		{
			MediaRequest = icyData;
			Server = server;
			Socket = socket;
			Client = client;
			Context = context;
		}
	}
}
