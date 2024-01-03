using Namiono.Common.Network.Sockets;
using Namiono.Network.Sockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Namiono.Common.Network
{
	public class ServerManager : IManager
	{
		public event ReceivedDataEventHandler ReceivedData;

		public Dictionary<Guid, INamionoServer> Servers { get; }

		public FileSystem FileSystem
		{
			get; set;
		}

		public ServerManager()
		{
			Servers = new Dictionary<Guid, INamionoServer>();
		}

		public void Start()
		{
			foreach (var server in Servers)
				server.Value.Start();
		}

		public void Send(Guid server, Guid socket, Guid client, byte[] data)
		{
			lock (Servers)
			{
				Servers[server].Sockets[socket].Send(client, data);
			}
		}

		public void Send(Guid server, Guid socket, Guid client, byte[] data, bool keepalive)
		{
			lock (Servers)
			{
				Servers[server].Sockets[socket].Send(client, data, keepalive);
			}
		}

		public void Send(Guid server, Guid socket, Guid client, MemoryStream data, bool keepalive)
		{
			lock (Servers)
			{
				Servers[server].Sockets[socket].Send(client, data, keepalive);
			}
		}

		public void Send(Guid server, Guid socket, Guid client, string data, Encoding encoding, bool keepalive)
		{
			lock (Servers)
			{
				Servers[server].Sockets[socket].Send(client, data, encoding, keepalive);
			}
		}

		public void Add(ProtoType protocolType, ServerMode mode, ushort port)
		{
			var guid = Guid.NewGuid();


			INamionoServer server;
			switch (protocolType)
			{
				case ProtoType.Tcp:
					server = new NamionoTcpServer(protocolType, guid, mode, port);
					break;
				case ProtoType.Raw:
				case ProtoType.Udp:
					server = new NamionoUdpServer(protocolType, guid, mode, port);
					break;
				default:
					throw new InvalidOperationException("No valid Server instance!");


			}
			NamionoCommon.Log("I", "ServerManager", string.Format("Added server on Port {0}!", port));
			server.ServerAddedSocket += (sender, e) =>
			{
				string.Format("Server '{0}' added Socket '{1}'", e.Server, e.Socket);

				Servers[e.Server].Sockets[e.Socket].Start();
			};

			server.ServerClosedSocket += (Sender, e) =>
			{
				NamionoCommon.Log("I", "ServerManager",
				   string.Format("Server '{0}' added Socket '{1}'", e.Server, e.Socket));
			};

			server.ServerClosedSocket += (Sender, e) =>
			{
				NamionoCommon.Log("I", "ServerManager",
					string.Format("Server '{0}' closed Socket '{1}'", e.Server, e.Socket));
			};

			server.ServerClosedClientConnection += (sender, e) =>
			{
				server.Sockets[e.Socket].Close(e.Client);

				NamionoCommon.Log("I", "ServerManager",
					string.Format("Client '{1}' dropped on Socket '{0}'!", e.Socket, e.Client));
			};

			server.ServerReceivedData += (sender, e) =>
			{
				switch (e.ServerMode)
				{
					case ServerMode.Http:
					case ServerMode.HttpMedia:
						using (var data = new MemoryStream(e.Data))
						{
							var context = new NamionoHttpContext(HttpProcessor.GetRequest(data, e.Data));
							context.Request.RemEndpoint = this.Servers[e.Server].Sockets[e.Socket].Clients[e.Client].RemoteEndpoint;
							
							ReceivedData.DynamicInvoke(this, new ReceivedDataArgs(e.Server, e.Socket, e.Client,
								Servers[e.Server].ProtocolType, Servers[e.Server].ServerMode, context));
						}

						break;
					default:
						break;
				}
			};

			if (!Servers.ContainsKey(guid))
				Servers.Add(guid, server);
		}

		public void Close()
		{
			lock (Servers)
			{
				foreach (var namionoServer in Servers.Values)
				{
					lock (namionoServer)
						namionoServer.Close();
				}
			}
		}

		public void Stop()
		{
			foreach (var namionoServer in Servers.Values)
				namionoServer.Stop();
		}

		public void Dispose()
		{
			foreach (var namionoServer in Servers.Values)
				namionoServer.Dispose();

			Servers.Clear();
		}

		public void HeartBeat()
		{
			foreach (var namionoServer in Servers.Values)
				namionoServer.HeartBeat();
		}

		public void Bootstrap() { }

		public delegate void ReceivedDataEventHandler(object sender, ReceivedDataArgs e);
	}
}
