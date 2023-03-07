using Namiono.Common.Network.Sockets;
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace Namiono.Common.Network
{
	public class NetworkManager : IManager
	{
		public delegate void NetworkManagerRequestHandledEventHandler(IManager sender,
			NetworkManagerRequestHandledEventArgs e);

		public delegate void HTTPRequestReceivedEventHandler(IManager sender, HTTPRequestReceivedEventArgs e);

		public event HTTPRequestReceivedEventHandler HTTPRequestReceived;

		public ServerManager ServerManager { get; }

		public FileSystem FileSystem { get; set; }

		public NetworkManager()
		{
			ServerManager = new ServerManager();
			ServerManager.ReceivedData += (sender, e) =>
			{
				switch (ServerManager.Servers[e.Server].ProtocolType)
				{
					case ProtoType.Tcp:
						switch (ServerManager.Servers[e.Server].ServerMode)
						{
							case ServerMode.HttpMedia:
							case ServerMode.Http:
								HTTPRequestReceived?.DynamicInvoke(this,
									new HTTPRequestReceivedEventArgs(e.Server, e.Socket, e.Client,
										ServerManager.Servers[e.Server].ServerMode == ServerMode.HttpMedia, e.Context));
								break;
							default:
								return;
						}
						break;
					case ProtoType.Udp:
						switch (ServerManager.Servers[e.Server].ServerMode)
						{
							case ServerMode.DHCP:
								break;
							case ServerMode.TFTP:
								break;
							default:
								return;
						}
						break;
					case ProtoType.Raw:
						break;
					default:
						return;
				}
			};
		}

		public static void GetIPAddresses(ServerMode mode, ushort port, Action<ServerMode, IPEndPoint> @delegate)
		{
			foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces()
				.Where(adap => adap.GetIPProperties().GatewayAddresses.Count != 0))
				foreach (var unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
					@delegate(mode, new IPEndPoint(unicastAddress.Address, port));
		}

		public void Close()
		{
			ServerManager.Close();
		}

		public void Start()
		{
			ServerManager.Start();
		}

		public void Stop()
		{
			ServerManager.Stop();
		}

		public void Dispose()
		{
			ServerManager.Dispose();
		}

		public void HeartBeat()
		{
			ServerManager.HeartBeat();
		}

		public void Bootstrap()
		{
			ServerManager.Bootstrap();
		}
	}
}
