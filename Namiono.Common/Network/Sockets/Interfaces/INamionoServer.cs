using Namiono.Common;
using Namiono.Common.Network.Sockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Namiono.Network.Sockets
{
	public interface INamionoServer : IDisposable
	{
		event ServerReceivedDataEventHandler ServerReceivedData;

		event ServerAddedSocketEventHandler ServerAddedSocket;

		event ServerClosedSocketEventHandler ServerClosedSocket;

		event ServerClosedClientConnectionEventHandler ServerClosedClientConnection;

		Dictionary<Guid, INamionoSocket> Sockets { get; set; }

		Guid Id { get; set; }

		ServerMode ServerMode { get; set; }

		ProtoType ProtocolType { get; set; }

		FileSystem FileSystem { get; set; }

		void Add(ServerMode mode, IPEndPoint endpoint);

		void Remove(Guid socket);
		void Start();

		void Close();

		void Send(Guid socket, Guid client, string data, Encoding encoding, bool keepAlive);
		void Send(Guid socket, Guid client, MemoryStream data, bool keepAlive);
		void Send(Guid socket, Guid client, byte[] data, bool keepAlive);
		void HeartBeat();
		void Stop();
		void Bootstrap();
	}
}
