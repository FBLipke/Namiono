using Namiono.Network.Sockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Namiono.Common.Network.Sockets
{
	public class NamionoTcpSocket : IDisposable, INamionoSocket
	{
		private TcpListener _sock;

		private event ClientAcceptedEventHandler InternalClientAccepted;

		public event SocketAddedClientEventHandler SocketAddedClient;

		public event SocketFailedToStartEventHandler SocketFailedToStart;

		public event SocketClosedClientEventHandler SocketClosedClient;

		public event SocketReadDataFromClientEventHandler SocketReadDataFromClient;

		public Dictionary<Guid, INamionoClient> Clients { get; set; }

		public IPEndPoint LocalEndpoint { get; }

		public Guid Id { get; set; }

		public bool Listening { get; set; }

		public NamionoTcpSocket(Guid id, IPEndPoint endpoint)
		{
			LocalEndpoint = endpoint;
			_sock = new TcpListener(LocalEndpoint);
			Clients = new Dictionary<Guid, INamionoClient>();
			Id = id;

			InternalClientAccepted += (sender, e) =>
			{
				Clients.Add(e.Client.Id, e.Client);

				SocketAddedClient.DynamicInvoke(this, new SocketAddedClientEventArgs(Id, e.Client.Id));
			};
		}

		public void Start()
		{
			try
			{
				_sock.Start();
				_sock.BeginAcceptTcpClient(new AsyncCallback(WaitForClients), null);
				Listening = true;

			}
			catch (SocketException ex)
			{
				SocketFailedToStart.DynamicInvoke(this, new SocketFailedToStartEventArgs(Id, ex));
			}
		}

		private void WaitForClients(IAsyncResult ar)
		{
			if (_sock == null || !Listening)
				return;

			var client = new NamionoTcpClient(Guid.NewGuid(), _sock.EndAcceptTcpClient(ar));
			client.ClientClosedConnection += (sender, e) => Remove(e.Client);
			client.ClientError += (sender, e) => Remove(e.Client);
			client.DataReadFromClient += (sender, e) =>
				SocketReadDataFromClient.DynamicInvoke
					(this, new SocketReadDataFromClientArgs(Id, e.Client, e.Data));

			InternalClientAccepted?.DynamicInvoke(this, new ClientAcceptedEventArgs(client));
			_sock.BeginAcceptTcpClient(new AsyncCallback(WaitForClients), null);
		}

		public void Send(Guid client, string data, Encoding encoding, bool keepAlive)
			=> Clients[client].Send(data, encoding, keepAlive);

		public void Close(Guid client)
		{
			if (!Clients.ContainsKey(client))
				return;

			Clients[client].Close();
			Remove(client);
		}

		public void Remove(Guid client)
		{
			if (!Clients.ContainsKey(client))
				return;
			Clients.Remove(client);

			SocketClosedClient?.DynamicInvoke(this, new SocketClosedClientEventArgs(client, Id));
		}

		public void Close()
		{
			Listening = false;

			lock (Clients.Values)
				foreach (var namionoTcpClient in Clients.Values)
					if (namionoTcpClient != null)
						lock (namionoTcpClient)
							namionoTcpClient.Close();

			_sock.Stop();
		}

		public void HeartBeat()
		{
			lock (Clients.Values)
				foreach (var namionoTcpClient in Clients.Values)
					if (!namionoTcpClient.Connected)
						Clients.Remove(namionoTcpClient.Id);
		}

		public void Send(Guid client, MemoryStream data, bool keepAlive)
		{
			if (Clients.ContainsKey(client))
				Clients[client].Send(data, keepAlive);
			else
				Clients.Remove(client);
		}

		public void Send(Guid client, byte[] data, bool keepAlive)
		{
			if (Clients.ContainsKey(client))
				Clients[client].Send(ref data, keepAlive);
			else
				Clients.Remove(client);
		}

		public void Dispose()
		{
			lock (Clients)
			{
				foreach (var namionoTcpClient in Clients.Values)
					lock (namionoTcpClient)
						namionoTcpClient.Dispose();

				Clients.Clear();
			}

			_sock = null;
		}

		public void Send(Guid client, byte[] data)
			=> Send(client, data, false);

		private class ClientAcceptedEventArgs
		{
			public ClientAcceptedEventArgs(INamionoClient client)
				=> Client = client;

			public INamionoClient Client { get; private set; }
		}

		private delegate void ClientAcceptedEventHandler(
		  INamionoSocket sender,
		  ClientAcceptedEventArgs e);

		public delegate void SocketAddedClientEventHandler(
		  INamionoSocket sender,
		  SocketAddedClientEventArgs e);

		public delegate void SocketFailedToStartEventHandler(
		  INamionoSocket sender,
		  SocketFailedToStartEventArgs e);

		public delegate void SocketClosedClientEventHandler(
		  INamionoSocket sender,
		  SocketClosedClientEventArgs e);

		public delegate void SocketReadDataFromClientEventHandler(
		  INamionoSocket sender,
		  SocketReadDataFromClientArgs e);
	}
}
