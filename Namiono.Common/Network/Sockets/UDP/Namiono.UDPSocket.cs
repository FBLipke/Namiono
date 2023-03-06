using Namiono.Network.Sockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Namiono.Common.Network.Sockets
{
	public class NamionoUdpSocket : IDisposable, INamionoSocket
	{
		protected class SocketState
		{
			public byte[] Buffer;
			public int Buffersize;
			public Socket Socket;
			public int Length;
			public SocketType Type;
		}

		private Socket _sock;

		private event ClientAcceptedEventHandler InternalClientAccepted;

		public event SocketAddedClientEventHandler SocketAddedClient;

		public event SocketFailedToStartEventHandler SocketFailedToStart;

		public event SocketClosedClientEventHandler SocketClosedClient;

		public event SocketReadDataFromClientEventHandler SocketReadDataFromClient;


		public Guid Id { get; set; }

		public bool Listening { get; set; }

		public Dictionary<Guid, INamionoClient> Clients { get; set; }

		bool INamionoSocket.Listening { get; set; }

		public EndPoint LocalEndpoint;
		SocketState state;


		public NamionoUdpSocket(Guid id, IPEndPoint endpoint)
		{
			LocalEndpoint = endpoint;
			_sock = new Socket(endpoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
			_sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
			_sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

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

				state = new SocketState();
				state.Buffer = new byte[1024];
				_sock.Bind(LocalEndpoint);

				_sock.BeginReceiveFrom(state.Buffer, 0, state.Buffer.Length, 0,
					ref LocalEndpoint, new AsyncCallback(Received), state);

				Listening = true;

			}
			catch (SocketException ex)
			{
				SocketFailedToStart.DynamicInvoke(this, new SocketFailedToStartEventArgs(Id, ex));
			}
		}

		private void Received(IAsyncResult ar)
		{
			if (_sock == null)
				return;

			state = (SocketState)ar.AsyncState;
			var clientSocket = state.Socket;

			var bytesRead = clientSocket.EndReceiveFrom(ar, ref LocalEndpoint);
			if (bytesRead == 0 || bytesRead == -1)
				return;

			var client = new NamionoUdpClient(Guid.NewGuid(), clientSocket);
			InternalClientAccepted?.DynamicInvoke(this, new ClientAcceptedEventArgs(client));

			SocketReadDataFromClient?.DynamicInvoke(this, new SocketReadDataFromClientArgs(Id, client.Id, state.Buffer));

			_sock.BeginReceiveFrom(state.Buffer, 0, state.Buffer.Length, 0,
				ref LocalEndpoint, new AsyncCallback(Received), state);
		}

		public void Send(Guid client, byte[] data) => Clients[client].Send(ref data);

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
			var socketClosedClient = SocketClosedClient;

			socketClosedClient?.DynamicInvoke(this, new SocketClosedClientEventArgs(client, Id));
		}

		public void Close()
		{
			Listening = false;
			lock (Clients.Values)
				foreach (var namionoClient in Clients.Values)
					if (namionoClient != null)
						lock (namionoClient)
							namionoClient.Close();

			_sock.Close();
		}

		public void HeartBeat()
		{
			lock (Clients.Values)
				foreach (var namionoClient in Clients.Values)
					if (!namionoClient.Connected)
						Clients.Remove(namionoClient.Id);
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
				Clients[client].Send(ref data);
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

		public void Send(Guid client, string data, Encoding encoding, bool keepAlive)
		{
			Clients[client].Send(data, encoding, keepAlive);
		}

		private class ClientAcceptedEventArgs
		{
			public ClientAcceptedEventArgs(INamionoClient client) => Client = client;

			public INamionoClient Client { get; private set; }
		}

		private delegate void ClientAcceptedEventHandler(object sender, ClientAcceptedEventArgs e);

		public delegate void SocketAddedClientEventHandler(object sender, SocketAddedClientEventArgs e);

		public delegate void SocketFailedToStartEventHandler(object sender, SocketFailedToStartEventArgs e);

		public delegate void SocketClosedClientEventHandler(object sender, SocketClosedClientEventArgs e);

		public delegate void SocketReadDataFromClientEventHandler(object sender, SocketReadDataFromClientArgs e);
	}
}
