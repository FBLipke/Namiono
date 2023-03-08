using Namiono.Module.Network.Protocols.DHCP;
using Namiono.Network;
using System;
using System.Collections.Generic;
using System.Net;

namespace Namiono.Module.Network.Protocols.DHCP
{
	public class DHCPPacket : Packet
	{
		public DHCPPacket(byte[] data) : base(data) { }

		public DHCPPacket(int length) : base(length) { }

		public byte Hops { get; private set; } = 0;

		public uint Xid { get; private set; } = 0;

		public ushort Secs { get; private set; } = 0;

		public IPAddress ClientIP { get; private set; } = IPAddress.Any;
		public IPAddress YourIP { get; private set; } = IPAddress.Any;

		public IPAddress ServerIP { get; private set; } = IPAddress.Any;

		public IPAddress RelayIP { get; private set; } = IPAddress.Any;

		public byte[] ClientMAC { get; private set; } = new byte[16];

		public byte[] ServerName { get; private set; } = new byte[64];

		public byte[] Bootfile { get; private set; } = new byte[128];

		public byte[] MagicCookie { get; private set; } = new byte[4];

		public Dictionary<byte, IDHCPOption> DHCPOptions;
	}
}
