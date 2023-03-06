using System;

namespace Namiono.Common.Network.Sockets
{
	public class DataReadFromClientEventArgs
	{
		public DataReadFromClientEventArgs(Guid id, byte[] data)
		{
			Data = data;
			Client = id;
		}

		public Guid Client { get; }

		public byte[] Data { get; }
	}
}
