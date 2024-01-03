using Namiono.Common;
using Namiono.Common.Network;
using Namiono.Common.Provider;
using Namiono.Database;
using System;
using System.Collections.Generic;

namespace Namiono.Module
{
	public class Memorymgr : IProvider, IManager
	{
		public Dictionary<Guid, IMember> Members { get; set; }

		public bool VolativeModule { get; set; } = true;

		public bool CanEdit { get; set; }

		public bool CanAdd { get; set; }

		public bool CanRemove { get; set; }

		public bool IsPublicModule { get; set; } = false;

		public string FriendlyName { get; set; } = "Speicherverwaltung";

		public string Description { get; set; } = "Gibt unbenutzten Speicher, welcher von Namiono nicht mehr gebraucht wird, wieder frei";

		public FileSystem FileSystem { get; set; }

		public bool Active { get; set; }

		public IDatabase Database
		{
			get;
			set;
		}

		public ICrypto Crypt { get; set; }

		public Memorymgr()
		{
			Members = new Dictionary<Guid, IMember>();
			Crypt = new MD5();
		}

		public void Bootstrap()
		{
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

		public IMember Get_Member(Guid id) => null;

		public void HeartBeat() => GC.Collect();

		[IsRuntimeModule]
		public void Install()
		{
		}

		public void Remove(Guid id)
		{
		}

		public IMember Request(Guid id) => null;

		public void Start()
		{
			HeartBeat();
			Active = true;
		}

		public void Stop()
		{
			if (!Active)
				return;
			Active = false;
			HeartBeat();
		}

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
