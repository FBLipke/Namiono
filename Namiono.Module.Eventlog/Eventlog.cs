// Decompiled with JetBrains decompiler
// Type: Namiono.Module.Eventlog
// Assembly: Namiono.Module.Eventlog, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EB206447-7AFD-4668-A541-6AFE81129AE2
// Assembly location: C:\Users\LipkeGu\Desktop\namiono___\Namiono.Module.Eventlog.dll

using Namiono.Common;
using Namiono.Common.Database;
using Namiono.Common.Network;
using Namiono.Common.Provider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Namiono.Module
{
	public class Eventlog : IProvider, IManager, ILog
	{
		public FileSystem Filesystem { get; set; }

		public SqlDatabase Database { get; set; }

		public Dictionary<Guid, IMember> Members { get; set; }

		public bool VolativeModule { get; set; } = true;

		public bool CanEdit { get; set; }

		public string FriendlyName { get; set; } = "Ereignisanzeige";

		public string Description { get; set; } = "Listet Meldungen welche zur Laufzeit auftreten auf.";

		public bool CanAdd { get; set; }

		public bool CanRemove { get; set; }

		public bool IsPublicModule { get; set; }

		public FileSystem FileSystem
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public bool Active { get; set; } = false;

		public Eventlog()
		{
			CanAdd = false;
			CanEdit = false;
			CanRemove = false;
			Members = new Dictionary<Guid, IMember>();
			Filesystem = new FileSystem("Providers\\Eventlog");
			if (VolativeModule)
				return;
			Database = new SqlDatabase(Filesystem, "Eventlog.db");
		}

		public void Bootstrap()
		{
			if (VolativeModule)
				return;
			Database?.Bootstrap();
		}

		public void Close()
		{
			if (VolativeModule)
				return;
			Database.Close();
		}

		public bool Contains(Guid id) => Members.ContainsKey(id);

		public void Dispose()
		{
			if (Database != null)
			{
				Database.Dispose();
				Database = null;
			}
			if (Members != null)
			{
				Members.Clear();
				Members = null;
			}
			Filesystem = null;
		}

		public string Handle_Get_Request(NamionoHttpContext request) => JsonConvert.SerializeObject(Members.Values);

		public void Install()
		{
		}

		public IMember Get_Member(Guid id) => Members.ContainsKey(id) ? Members[id] : null;

		public void HeartBeat()
		{
			if (VolativeModule)
				return;
			Database?.HeartBeat();
			Update();
		}

		public void Remove(Guid id) => Members.Remove(id);

		public IMember Request(Guid id) => Members[id];

		public void Start()
		{
			Active = true;
			NamionoCommon.Log("I", FriendlyName, " Following messages from Console \"" + Console.Title + "\" will redirected to this Module!");
		}

		public void Stop()
		{
			Active = false;
			Provider.Commit(Members, FriendlyName, Database, Filesystem);
		}

		public void Update()
		{
			if (!Active)
				return;
			Provider.Commit(Members, FriendlyName, Database, Filesystem);
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
