using System;

namespace Namiono.Common.Network
{
	public class NetworkDataReceivedEventArgs<T>
	{
		public Guid Server { get; private set; }

		public Guid Socket { get; private set; }

		public Guid Client { get; private set; }

		public Origin Source { get; private set; }

		public Origin Target { get; private set; }

		public T Data { get; private set; }

		public NetworkDataReceivedEventArgs(
		  Guid server,
		  Guid socket,
		  Guid client,
		  Origin src,
		  Origin dest,
		  T data)
		{
			Data = data;
			Server = server;
			Socket = socket;
			Client = client;
			Source = src;
			Target = dest;
		}
	}
}
