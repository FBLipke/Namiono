using Namiono.Network.Sockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Namiono.Common.Network.Sockets
{
	public class NamionoTcpServer : IDisposable, IManager, INamionoServer
	{
		public Dictionary<Guid, INamionoSocket> Sockets { get; set; }

		public Guid Id { get; set; }

		public ServerMode ServerMode { get; set; }

		public ProtoType ProtocolType { get; set; }

		public FileSystem FileSystem { get; set; }



		Action<ServerMode, IPEndPoint> YieldFunc => (mode, endp) =>
		{
			Add(mode, endp);
		};

		public NamionoTcpServer(ProtoType protocolType, Guid id, ServerMode mode, ushort port)
		{
			Id = id;
			ProtocolType = protocolType;
			Sockets = new Dictionary<Guid, INamionoSocket>();
			ServerMode = mode;

			NetworkManager.GetIPAddresses(ServerMode, port, YieldFunc);
		}

		public event Sockets.ServerAddedSocketEventHandler ServerAddedSocket;

		public event Sockets.ServerClosedSocketEventHandler ServerClosedSocket;

		public event Sockets.ServerClosedClientConnectionEventHandler ServerClosedClientConnection;

		public event Sockets.ServerReceivedDataEventHandler ServerReceivedData;

		public void Add(ServerMode mode, IPEndPoint endpoint)
		{
			var guid = Guid.NewGuid();
			var namionoTcpSocket = new NamionoTcpSocket(guid, endpoint);
			namionoTcpSocket.SocketAddedClient += (sender, e) => Sockets[e.SocketId].Clients[e.ClientId]?.Read();
			namionoTcpSocket.SocketFailedToStart += (sender, e) => Remove(e.Socket);
			namionoTcpSocket.SocketClosedClient += (sender, e) =>
		   {
			   ServerClosedSocket(this, new ServerClosedSocketArgs(Id, e.Socket));
			   ServerClosedClientConnection(this, new ServerClosedClientConnectionArgs(Id, e.Socket, e.Client));
		   };

			namionoTcpSocket.SocketReadDataFromClient += (sender, e) =>
			{
				ServerReceivedData.DynamicInvoke(this,
					new ServerReceivedDataArgs(mode, ProtocolType, Id, e.Socket, e.Client, e.Data));
			};

			Sockets.Add(guid, namionoTcpSocket);

			ServerAddedSocket?.DynamicInvoke(this, new ServerAddedSocketArgs(Id, guid));
		}

		public void Remove(Guid socket)
		{
			if (!Sockets.ContainsKey(socket))
				return;

			Sockets.Remove(socket);

			ServerClosedSocket(this, new ServerClosedSocketArgs(Id, socket));
		}

		public void Start()
		{
			foreach (var namionoTcpSocket in Sockets.Values)
				namionoTcpSocket.Start();
		}

		public void Close()
		{
			foreach (var namionoTcpSocket in Sockets.Values)
				namionoTcpSocket.Close();
		}

		public void Dispose()
		{
			foreach (var namionoTcpSocket in Sockets.Values)
				namionoTcpSocket.Dispose();

			Sockets.Clear();
		}

		public void Send(Guid socket, Guid client, string data, Encoding encoding, bool keepAlive)
			=> Sockets[socket].Send(client, data, encoding, keepAlive);

		public void Send(Guid socket, Guid client, MemoryStream data, bool keepAlive)
			=> Sockets[socket].Send(client, data, keepAlive);

		public void Send(Guid socket, Guid client, byte[] data, bool keepAlive)
			=> Sockets[socket].Send(client, data, keepAlive);

		public void Stop()
		{
			foreach (var socket in Sockets.Values)
				socket.Close();
		}

		public void HeartBeat()
		{
			lock (Sockets)
			{
				Guid socket = Guid.Empty;
				if (!Sockets.Values.Where(s => !s.Listening).Any())
					return;
				using (var enumerator = Sockets.Values.Where(s => !s.Listening).GetEnumerator())
				{
					if (enumerator.MoveNext())
						socket = enumerator.Current.Id;
				}
				Remove(socket);
			}
		}

		public void Bootstrap() => throw new NotImplementedException();

		public delegate void ServerReceivedDataEventHandler(
		  INamionoServer sender,
		  ServerReceivedDataArgs e);

		public delegate void ServerAddedSocketEventHandler(
		  INamionoServer sender,
		  ServerAddedSocketArgs e);

		public delegate void ServerClosedSocketEventHandler(
		  INamionoServer sender,
		  ServerClosedSocketArgs e);

		public delegate void ServerClosedClientConnectionEventHandler(
		  INamionoServer sender,
		  ServerClosedClientConnectionArgs e);
	}
}
