using System;

namespace Namiono.Common.Network
{
	public partial class ReceivedDataArgs : EventArgs
	{
		public NamionoHttpContext Context { get; private set; }

		public Guid Server { get; private set; }

		public Guid Socket { get; private set; }

		public Guid Client { get; private set; }

		public byte[] Data { get; private set; }

		public ReceivedDataArgs(Guid server, Guid socket, Guid client, byte[] data)
		{
			Server = server;
			Socket = socket;
			Client = client;
			Data = data;
		}

		public ReceivedDataArgs(
		  Guid server,
		  Guid socket,
		  Guid client,
		  NamionoHttpContext httpcontext)
		{
			Server = server;
			Socket = socket;
			Client = client;
			Context = httpcontext;
		}
	}
}
