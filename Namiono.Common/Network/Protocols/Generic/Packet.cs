using System;

namespace Namiono.Network
{
	public class Packet
	{
		public byte[] Payload { get; private set; }

		public int Position { get; private set; }

		public int Length { get; private set; }

		public Packet(int length)
		{
			Position = 0;
			Payload = new byte[length];
			Length = length;
		}

		/// <summary>
		/// Set Position in the Packet Buffer
		/// </summary>
		/// <param name="position"></param>
		/// <returns>The current position in the Packet</returns>
		public int SetPosition(int position)
			=> Position = position;


		public Packet(byte[] data)
		{
			Payload = new byte[data.Length];
			Array.Copy(data, Payload, data.Length);
			Position = Payload.Length;
			Length = Payload.Length;
		}

		/// <summary>
		/// Write data into the Packet
		/// </summary>
		/// <param name="data">The data to be written</param>
		/// <param name="length">Length of the source data</param>
		public void Write(byte[] data, int length = 0)
		{
			var len = length == 0 ? data.Length : length;

			if (len == 0)
			{
				if ((Position + len) >= Length)
					throw new IndexOutOfRangeException("Packet::Write(2): Not enough space in Packet!" +
						"((Position + NewData.Length) > PacketLength)!");
			}

			Array.Copy(data, 0, Payload, Position, len);
			Position += len;
		}

		/// <summary>
		/// Read data from the Packet
		/// </summary>
		/// <param name="position">Position to start in the Packet.</param>
		/// <param name="length">How many bytes needs to be read (0 means entire Packet Length)</param>
		/// <returns>the requested data which was read from the Packet</returns>
		public byte[] Read(int position, int length = 0)
		{
			var target = new byte[length == 0 ? Payload.Length : length];
			var curPos = Position;

			SetPosition(position);
			Array.Copy(Payload, Position, target, 0, target.Length);
			SetPosition(curPos);

			return target;
		}
	}
}
