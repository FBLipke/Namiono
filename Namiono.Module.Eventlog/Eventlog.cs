using Namiono.Common;
using Namiono.Common.Database;
using Namiono.Common.Network;
using Namiono.Common.Provider;
using Namiono.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Namiono.Module
{
	public class Eventlog : IProvider, IManager, ILog
	{
		public FileSystem Filesystem { get; set; }

		public IDatabase Database { get; set; }

		public Dictionary<Guid, IMember> Members { get; set; }

		public bool VolativeModule { get; set; } = true;

		public bool CanEdit { get; set; }

		public string FriendlyName { get; set; } = "Ereignisanzeige";

		public string Description { get; set; } = "Listet Meldungen welche zur Laufzeit auftreten auf.";

		public bool CanAdd { get; set; }

		public bool CanRemove { get; set; }

		public bool IsPublicModule { get; set; }

		public FileSystem FileSystem { get; set; }

		public bool Active { get; set; } = false;

		public ICrypto Crypt { get; set; }

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

		public void Install() {	}

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
			return string.Empty;
		}

		public string Handle_Edit_Request(NamionoHttpContext context)
		{
			return string.Empty;
		}

		public string Handle_Remove_Request(NamionoHttpContext context)
		{
			return string.Empty;
		}

		public string Handle_Info_Request(NamionoHttpContext context)
		{
			return string.Empty;
		}

		public void Log(string type, string name, string logmessage)
		{
			var str = "\t" + DateTime.Now.ToString("dd.MM.yyyy : HH:mm:ss", CultureInfo.InvariantCulture)
				+ "\tNamiono." + name + ": " + logmessage;

			var key = Guid.NewGuid();
			var num = DateTime.Now.AsUnixTimeStamp();

			Members.Add(key, new Member()
			{
				Name = name,
				Id = key,
				Description = str,
				Author = Guid.Empty,
				Created = num,
				Updated = num,
				Provider = name,
				Url = "-"
			});

			Console.WriteLine("[" + type + "]" + str);
		}
	}
}
