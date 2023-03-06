using Namiono.Network.Sockets;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Namiono.Common.Network.Sockets
{
	public class NamionoUdpClient : IDisposable, INamionoClient
	{
		public event ClientErrorEventHandler ClientError;
		public Socket Socket;

		public NamionoUdpClient(Guid id, Socket socket)
		{
			Socket = socket;
			Id = id;
			Connected = true;
		}

		public bool Connected { get; set; }
		public Guid Id { get; set; }
		public IPEndPoint RemoteEndpoint { get; set; }

		public void Close()
		{
			Socket.Close();
			Connected = false;
		}

		public void Dispose()
		{
			Socket.Dispose();
		}

		public void Send(ref byte[] data)
		{
			Socket.BeginSendTo(data, 0, data.Length,
				SocketFlags.None, Socket.RemoteEndPoint, new AsyncCallback(EndSend), Socket);
		}
		public void Send(ref MemoryStream data)
		{
			var x = data.GetBuffer();
			Send(ref x);
		}

		private void EndSend(IAsyncResult ar)
		{
			using (var so = (Socket)ar.AsyncState)
			{
				var bytesSend = so.EndSendTo(ar);

				if (bytesSend == 0 || bytesSend == -1)
				{
					ClientError?.DynamicInvoke(this, new ClientErrorEventArgs(Id, null));
					return;
				}
			}
		}

		public void HeartBeat()
		{
		}

		public void Start()
		{
		}

		public void Disconnect()
		{
		}

		public void Send(string data, bool keepAlive)
		{
			var x = Encoding.ASCII.GetBytes(data);
			Send(ref x);
		}

		public void Send(MemoryStream data, bool keepAlive)
		{
			Send(ref data);
		}

		public void Send(string data, Encoding encoding, bool keepAlive)
		{
			var x = encoding.GetBytes(data);
			Send(ref x);
		}

		public void Send(ref byte[] data, bool keepalive)
		{
			Send(ref data);
		}

		public void Read()
		{
			throw new NotImplementedException();
		}

		public delegate void DataReadFromClientEventHandler(object sender, DataReadFromClientEventArgs e);
		public delegate void ClientErrorEventHandler(object sender, ClientErrorEventArgs e);
		public delegate void ClientClosedEventHandler(object sender, ClientConnectionClosedEventArgs e);
		public delegate void ClientConnectedEventHandler(object sender, ClientConnectedEventArgs e);
	}
}
