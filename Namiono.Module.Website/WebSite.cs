
using Namiono.Common;
using Namiono.Common.Database;
using Namiono.Common.Network;
using Namiono.Common.Provider;
using System;
using System.Collections.Generic;

namespace Namiono.Module
{
	public class Website : IProvider, IManager
	{
		public Dictionary<Guid, IMember> Members { get; set; }

		public bool VolativeModule { get; set; } = true;

		public bool CanEdit { get; set; }

		public string FriendlyName { get; set; } = "Icy Relay";

		public string Description { get; set; } = "Stellt einen rudimentären ICY Media Stream bereit.";

		public bool CanAdd { get; set; }

		public bool CanRemove { get; set; }

		public bool IsPublicModule { get; set; } = false;

		public FileSystem FileSystem { get; set; }

		public bool Active { get; set; }

		public SqlDatabase Database { get; set; }

		public Website() => Members = new Dictionary<Guid, IMember>();

		public void Bootstrap()
		{
			NamionoCommon.NetworkManager.ServerManager
				.Add(Common.Network.Sockets.ProtoType.Tcp,
				Common.Network.Sockets.ServerMode.Http, 90);

			NamionoCommon.NetworkManager.HTTPRequestReceived += (sender, e) =>
			{
				Handle_HTTP_Request(e.Server, e.Socket, e.Client, e.Context);
			};
		}

		public void Close()
		{

		}

		public bool Contains(Guid id) => false;

		public void Dispose()
		{
			Members.Clear();
			Members = null;
		}

		public IMember Get_Member(Guid id) => (IMember)null;

		public void Handle_HTTP_Request(Guid server, Guid socket, Guid client, NamionoHttpContext context)
		{

			NamionoCommon.NetworkManager.ServerManager.Send(server, client, socket, new byte[0]);
		}

		public void HeartBeat()
		{
		}

		public void Install()
		{
		}

		public void Remove(Guid id)
		{
		}

		public IMember Request(Guid id) => (IMember)null;

		public void Start() => HeartBeat();

		public void Stop() => HeartBeat();

		public string Handle_Get_Request(NamionoHttpContext context)
		{
			throw new NotImplementedException();
		}

		public string Handle_Add_Request(NamionoHttpContext context)
		{
			throw new NotImplementedException();
		}

		public string Handle_Edit_Request(NamionoHttpContext context)
		{
			throw new NotImplementedException();
		}

		public string Handle_Remove_Request(NamionoHttpContext context)
		{
			throw new NotImplementedException();
		}

		public string Handle_Info_Request(NamionoHttpContext context)
		{
			throw new NotImplementedException();
		}
	}
}
