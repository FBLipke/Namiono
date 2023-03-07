using System;

namespace Namiono.Module.Network.Protocols.DHCP
{
	public class DHCPOption
	{
		public byte Option { get; private set; }

		public byte Length { get; private set; }

		public byte[] Data { get; private set; }

		public DHCPOption(byte number, byte length, byte[] data)
		{
			Option = number;
			Length = length;
			Data = data;
		}


		public byte[] GetBytes(DHCPOption option)
		{
			var bytes = new byte[(option.Length + 2)];
			bytes[0] = option.Option;
			bytes[1] = option.Length;
			Array.Copy(option.Data, 0, bytes, 2, option.Length);

			return bytes;
		}
	}
}
