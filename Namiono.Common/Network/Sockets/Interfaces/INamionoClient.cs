using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Namiono.Network.Sockets
{
	public interface INamionoClient : IDisposable
	{
		Guid Id { get; set; }
		bool Connected { get; set; }
		IPEndPoint RemoteEndpoint { get; set; }
		void HeartBeat();
		void Send(ref byte[] data, bool keepalive);
		void Send(ref byte[] data);
		void Send(string data, bool keepAlive);
		void Send(MemoryStream data, bool keepAlive);
		void Send(string data, Encoding encoding, bool keepAlive);
		void Close();
		void Start();
		void Disconnect();
		void Read();
	}
}
