using Namiono.Network.Sockets;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Namiono.Common.Network.Sockets
{
	public partial class NamionoTcpClient : INamionoClient
	{
		private TcpClient _client;

		public event DataReadFromClientEventHandler DataReadFromClient;

		public event ClientErrorEventHandler ClientError;

		public void Heartbeat()
		{
		}

		public void Start()
		{
			if (Connected)
				return;
			Connect(RemoteEndpoint);
		}

		public event ClientClosedEventHandler ClientClosedConnection;

		public event ClientConnectedEventHandler ClientConnected;

		public NamionoTcpClient(Guid id, TcpClient c)
		{
			Id = id;
			_client = c;
			RemoteEndpoint = (IPEndPoint)_client.Client.RemoteEndPoint;
			InputStream = OutputStream = new BufferedStream(_client.GetStream());
			Connected = _client.Connected;
		}

		public NamionoTcpClient(Guid id, string host, ushort port)
		{
			Id = id;
			try
			{
				_client = new TcpClient(host, port);
				Connected = _client.Connected;
			}
			catch (SocketException ex)
			{
				ClientError.DynamicInvoke(this, new ClientErrorEventArgs(Id, ex));
			}
		}

		public BufferedStream OutputStream { get; set; }

		public BufferedStream InputStream { get; set; }


		public Guid Id { get; set; }
		public bool Connected { get; set; }
		public IPEndPoint RemoteEndpoint { get; set; }

		public void Send(string data, Encoding encoding, bool keepAlive)
			=> Send(encoding.GetBytes(data), keepAlive);

		public void Read()
		{
			if (!Connected || !InputStream.CanRead)
			{
				Close();
			}
			else
			{
				var array = new byte[16384];
				var newSize = InputStream.Read(array, 0, array.Length);
				if (newSize == 0 || newSize == -1)
				{
					Close();
				}
				else
				{
					Array.Resize(ref array, newSize);
					DataReadFromClient.DynamicInvoke(this, new DataReadFromClientEventArgs(Id, array));
				}
			}
		}

		public void Connect(string host, ushort port)
			=> _client.BeginConnect(host, port, new AsyncCallback(EndConnect), null);

		public void Connect(IPEndPoint endPoint)
		{
			try
			{
				_client.BeginConnect(endPoint.Address, endPoint.Port, new AsyncCallback(EndConnect), null);
			}
			catch (Exception ex)
			{
				ClientError.DynamicInvoke(this, new ClientErrorEventArgs(Id, ex));
			}
		}

		private void EndConnect(IAsyncResult ar)
		{
			_client.EndConnect(ar);
			Connected = _client.Connected;
			if (!Connected)
			{
				_client.Close();
			}
			else
			{
				InputStream = OutputStream = new BufferedStream(_client.GetStream());

				ClientConnected.DynamicInvoke(this, new ClientConnectedEventArgs(Id));
			}
		}

		public void Disconnect() => Close();

		public void Send(MemoryStream stream, bool keepAlive) => Send(stream.ToArray(), keepAlive);

		public void Send(byte[] data, bool keepAlive)
		{
			if (!OutputStream.CanWrite)
				return;

			OutputStream.BeginWrite(data, 0, data.Length, new AsyncCallback(EndWriteData), keepAlive);
		}

		public void Close()
		{
			InputStream?.Close();
			OutputStream?.Close();
			if (_client != null)
			{
				_client.Close();
				_client = null;
			}
			Connected = false;

			ClientClosedConnection.DynamicInvoke
				(this, new ClientConnectionClosedEventArgs(Id));
		}

		public void EndWriteData(IAsyncResult ar)
		{
			if (OutputStream.CanWrite)
			{
				OutputStream?.EndWrite(ar);
				OutputStream?.FlushAsync();
			}

			Close();
		}

		public void Dispose()
		{
			if (InputStream != null)
			{
				InputStream.Dispose();
				InputStream = null;
			}

			if (OutputStream != null)
			{
				OutputStream.Dispose();
				OutputStream = null;
			}

			if (_client == null)
				return;

			_client.Close();
			_client = null;
		}

		public void Send(ref byte[] data)
		{
			Send(data, true);
		}

		public void Send(ref MemoryStream data)
		{
			Send(data, true);
		}

		public void HeartBeat()
		{
		}

		public void Send(string data, bool keepAlive)
		{
			Send(Encoding.ASCII.GetBytes(data), keepAlive);
		}

		public void Send(ref byte[] data, bool keepalive)
		{
			Send(data, keepalive);
		}

		public delegate void DataReadFromClientEventHandler(object sender, DataReadFromClientEventArgs e);
		public delegate void ClientErrorEventHandler(object sender, ClientErrorEventArgs e);
		public delegate void ClientClosedEventHandler(object sender, ClientConnectionClosedEventArgs e);
		public delegate void ClientConnectedEventHandler(object sender, ClientConnectedEventArgs e);
	}
}
