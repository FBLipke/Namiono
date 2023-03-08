using Namiono.Common.Network.Sockets;
using System;

namespace Namiono.Common.Network
{
	public partial class ReceivedDataArgs : EventArgs
	{
		public NamionoHttpContext Context { get; private set; }

		public Guid Server { get; private set; }

		public Guid Socket { get; private set; }

		public ServerMode ServerMode { get; private set; }

		public ProtoType ProtoType { get; private set; }

		public Guid Client { get; private set; }

		public byte[] Data { get; private set; }

		public ReceivedDataArgs(Guid server, Guid socket, Guid client, 
			ProtoType type, ServerMode serverMode, byte[] data)
		{
			Server = server;
			Socket = socket;
			Client = client;
			ProtoType = type;
			Data = data;
		}

		public ReceivedDataArgs(
		  Guid server,
		  Guid socket,
		  Guid client,
		  ProtoType type, ServerMode serverMode,
		  NamionoHttpContext httpcontext)
		{
			Server = server;
			Socket = socket;
			Client = client;
			Context = httpcontext;
		}
	}
}
