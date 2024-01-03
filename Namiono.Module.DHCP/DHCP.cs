using Namiono.Common;
using Namiono.Common.Database;
using Namiono.Common.Network;
using Namiono.Common.Network.Sockets;
using Namiono.Common.Provider;
using Namiono.Database;
using Namiono.Module.Network.Protocols.DHCP;
using System;
using System.Collections.Generic;

namespace Namiono.Module
{
	public class DHCP : IProvider, IManager
	{
		public DHCP()
		{
			CanAdd = false;
			CanEdit = false;
			CanRemove = false;
			Members = new Dictionary<Guid, IMember>();
			FileSystem = new FileSystem("Providers\\DHCP");
			Database = new SqlDatabase(FileSystem, "DHCP.db");
		}

		public FileSystem FileSystem { get; set; }

		public IDatabase Database { get; set; }

		public Dictionary<Guid, IMember> Members { get; set; }

		public bool VolativeModule { get; set; } = false;

		public bool CanEdit { get; set; }

		public string FriendlyName { get; set; } = "PXEService";

		public string Description { get; set; } = "Stellt Dienste, welche das Starten über PXE ermöglichen, bereit.";

		public bool CanAdd { get; set; }

		public bool CanRemove { get; set; }

		public bool IsPublicModule { get; set; }
		public ICrypto Crypt { get; set; }
		public bool Active { get; set; }

		public void Bootstrap()
		{
			NamionoCommon.NetworkManager.ServerManager.ReceivedData += (sender, e) =>
			{
				if (e.ProtoType == ProtoType.Tcp)
					return;

				if (e.ServerMode != ServerMode.DHCP
				|| e.ServerMode != ServerMode.BOOTP)
					return;
			};
		}

		public void Close() {}

		public bool Contains(Guid id) { return false; }

		public void Dispose() {	}

		public string Handle_Add_Request(NamionoHttpContext context)
		{
			return string.Empty;
		}

		public void Handle_Discover_Packet(Guid server, Guid socket, Guid client, DHCPPacket packet)
		{
		}

		public string Handle_Edit_Request(NamionoHttpContext context)
		{
			return string.Empty;
		}

		public string Handle_Get_Request(NamionoHttpContext context)
		{
			return string.Empty;
		}

		public string Handle_Info_Request(NamionoHttpContext context)
		{
			return string.Empty;
		}

		public string Handle_Remove_Request(NamionoHttpContext context)	{ return string.Empty; }

		public void Handle_Request_Packet(Guid server, Guid socket, Guid client, DHCPPacket packet) { }

		public void HeartBeat() { }

		public void Install() => Provider.Install(nameof(DHCP), Members, Database, FileSystem, Crypt);

		public void Remove(Guid id) { }

		public IMember Request(Guid id) { return null; }

		public void Start()
		{
			Active = true;
		}

		public void Stop() { }
	}
}
