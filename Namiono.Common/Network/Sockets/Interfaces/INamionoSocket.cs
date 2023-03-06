using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Namiono.Network.Sockets
{
	public interface INamionoSocket : IDisposable
	{
		Dictionary<Guid, INamionoClient> Clients { get; set; }

		Guid Id { get; set; }

		bool Listening { get; set; }

		void Start();

		void Send(Guid client, byte[] data);
		void Send(Guid client, MemoryStream data, bool keepAlive);
		void Send(Guid client, byte[] data, bool keepAlive);
		void Send(Guid client, string data, Encoding encoding, bool keepAlive);

		void Close(Guid client);

		void Remove(Guid client);

		void Close();

		void HeartBeat();
	}
}
