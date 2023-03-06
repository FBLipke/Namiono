// Decompiled with JetBrains decompiler
// Type: Namiono.Module.Memorymgr
// Assembly: Namiono.Module.Memorymgr, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E71C7487-B14F-4DC1-9DD9-A4472484B999
// Assembly location: C:\Users\LipkeGu\Desktop\namiono___\Namiono.Module.Memorymgr.dll

using Namiono.Common;
using Namiono.Common.Database;
using Namiono.Common.Network;
using Namiono.Common.Provider;
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

		public FileSystem FileSystem
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public bool Active { get; set; }

		public SqlDatabase Database
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public Memorymgr() => Members = new Dictionary<Guid, IMember>();

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

		public IMember Get_Member(Guid id) => (IMember)null;

		public void HeartBeat() => GC.Collect();

		[IsRuntimeModule]
		public void Install()
		{
		}

		public void Remove(Guid id)
		{
		}

		public IMember Request(Guid id) => (IMember)null;

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
