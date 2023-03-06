using Namiono.Common.Network.Sockets;
using Namiono.Network.Sockets;
using System;
using System.Collections.Generic;

namespace Namiono.Common.Network
{
	public class ClientManager : IManager
	{
		public event ClientManagerReceivedDataEventHandler ClientManagerReceivedData;

		public event ClientManagerClosedConnectionEventHandler ClientManagerClosedConnection;

		public Dictionary<Guid, INamionoClient> Clients { get; }

		public FileSystem FileSystem { get; set; }

		public ClientManager()
		{
			Clients = new Dictionary<Guid, INamionoClient>();
			HttpRequest httpRequest = new HttpRequest()
			{
				Method = "POST",
				Path = "/api",
				Version = "HTTP/1.1"
			};
			httpRequest.Headers.Add("User-Agent", "Mozilla/5.0");
			httpRequest.Headers.Add("Accept", "*/*");
			httpRequest.Headers.Add("Content-Encoding", "application/x-www-form-urlencoded; charset=utf-8");
			httpRequest.Headers.Add("Connection", "close");
			httpRequest.Create();
		}

		public void Add(string host, ushort port)
		{
			var key = Guid.NewGuid();
			var namionoTcpClient = new NamionoTcpClient(Guid.NewGuid(), host, port);

			namionoTcpClient.DataReadFromClient += (sender, e) =>
			{
				var managerReceivedData = ClientManagerReceivedData;

				managerReceivedData(this, new ClientManagerReceivedDataArgs(e.Client, e.Data));
			};
			namionoTcpClient.ClientError += (sender, e) =>
			{
				if (!Clients.ContainsKey(e.Client))
				{
					NamionoCommon.Log("W", "Namiono.ClientManager", string.Format("Client ({0}) requested but does not exist anymore!", e.Client));
				}
				else
				{
					NamionoCommon.Log("W", "Namiono.ClientManager", string.Format("Client ({0}) removed due to errors!", e.Client));
					Clients.Remove(e.Client);
					ClientManagerClosedConnection.DynamicInvoke(this, new ClientManagerClosedConnectionArgs(e.Client));
				}
			};

			if (!Clients.ContainsKey(key))
				Clients.Add(key, namionoTcpClient);

			namionoTcpClient.ClientClosedConnection += (sender, e) =>
			{
				NamionoCommon.Log("W", "Namiono.ClientManager", string.Format("Client ({0}) removed!", e.Client));
				Clients.Remove(e.Client);
			};

			Start();
		}

		public void Close()
		{
			foreach (var namionoTcpClient in Clients.Values)
				namionoTcpClient.Close();
		}

		public void Dispose()
		{
			foreach (var namionoTcpClient in Clients.Values)
				namionoTcpClient.Dispose();
		}

		public void HeartBeat()
		{
			foreach (var namionoTcpClient in Clients.Values)
				namionoTcpClient.HeartBeat();
		}

		public void Start()
		{
			foreach (var namionoTcpClient in Clients.Values)
				namionoTcpClient.Start();
		}

		public void Stop()
		{
			foreach (var namionoTcpClient in Clients.Values)
				namionoTcpClient.Disconnect();
		}

		public void Bootstrap() => throw new NotImplementedException();

		public delegate void ClientManagerReceivedDataEventHandler(
		  IManager sender,
		  ClientManagerReceivedDataArgs e);

		public delegate void ClientManagerClosedConnectionEventHandler(
		  IManager sender,
		  ClientManagerClosedConnectionArgs e);
	}
}
