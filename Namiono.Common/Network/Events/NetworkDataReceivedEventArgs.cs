using System;

namespace Namiono.Common.Network
{
	public class NetworkDataReceivedEventArgs<T>
	{
		public Guid Server { get; private set; }

		public Guid Socket { get; private set; }

		public Guid Client { get; private set; }

		public T Data { get; private set; }

		public NetworkDataReceivedEventArgs(
		  Guid server,
		  Guid socket,
		  Guid client,
		  T data)
		{
			Data = data;
			Server = server;
			Socket = socket;
			Client = client;
		}
	}
}
