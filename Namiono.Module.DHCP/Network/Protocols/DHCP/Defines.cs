using System;

namespace Namiono.Module.Network.Protocols.DHCP
{
	public enum BOOTPOPCode : byte
	{
		Request = 0x01,
		Reply = 0x02
	}

	public enum BOOTPHWType
	{
		Ethernet = 0x01
	}

	public enum BOOTPHWLen
	{
		Ethernet = 0x06
	}

	public class DHCPOption : IDHCPOption
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

		public byte[] GetBytes()
		{
			var bytes = new byte[(Length + 2)];
			bytes[0] = Option;
			bytes[1] = Length;
			Array.Copy(Data, 0, bytes, 2, Length);

			return bytes;
		}

	}
}
