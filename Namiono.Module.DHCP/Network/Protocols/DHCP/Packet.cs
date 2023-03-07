using Namiono.Network;

namespace Namiono.Module.DHCP.Network.Protocols.DHCP
{
	public class DHCPPacket : Packet
	{
		DHCPPacket(byte data) : base(data) { }
		DHCPPacket(int length) : base(length) { }
	}
}
