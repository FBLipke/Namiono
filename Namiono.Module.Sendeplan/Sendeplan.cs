using Namiono.Common;
using Namiono.Common.Database;
using Namiono.Common.Network;
using Namiono.Common.Provider;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Namiono.Module
{
	public class Sendeplan : IProvider, IManager
	{
		public bool VolativeModule { get; set; } = false;

		public Dictionary<Guid, IMember> Members { get; set; }

		public FileSystem FileSystem { get; set; }

		public bool CanEdit { get; set; } = true;

		public string FriendlyName { get; set; } = nameof(Sendeplan);

		public string Description { get; set; }

		public bool IsPublicModule { get; set; } = true;

		public bool CanAdd { get; set; } = true;

		public bool CanRemove { get; set; } = true;

		public bool Active { get; set; } = true;

		public SqlDatabase Database { get; set; }

		public Sendeplan()
		{
			Members = new Dictionary<Guid, IMember>();
			FileSystem = new FileSystem("Providers\\Sendeplan");
			Database = new SqlDatabase(FileSystem, "Sendeplan.db");
		}

		public void Bootstrap() => Database.Bootstrap();

		public void Close() => Database.Close();

		public bool Contains(Guid id) => throw new NotImplementedException();

		public void Dispose() => Database.Dispose();

		public IMember Get_Member(Guid id) => !Members.ContainsKey(id) ? null : Members[id];

		private string Generate_Entry(DateTime hour)
		{
			var stringBuilder = new StringBuilder();

			if (Members.Count == 0)
				return stringBuilder.ToString();

			Members.Values.Where(e => DateTime.Compare(hour, DateTime.Parse(e.Created.AsString())) == 0).FirstOrDefault();

			return stringBuilder.ToString();
		}

		private string Generáte_Day(DateTime day)
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.Append("<ul>");
			for (int index = 16; index <= 22; ++index)
			{
				var hour = day.AddHours(index);
				stringBuilder.Append("<li class=\"sendeplan_entry\">");
				stringBuilder.Append("<ul>");
				stringBuilder.AppendFormat("<li>{0}</li>", hour.ToString("HH:mm"));
				stringBuilder.AppendFormat("<li>{0}</li>", Generate_Entry(hour));
				stringBuilder.Append("</ul>");
				stringBuilder.Append("</li>");
			}
			stringBuilder.Append("</ul>");
			return stringBuilder.ToString();
		}

		DateTime FirstDayOfWeek()
		{
			var dayOfWeek = (int)DateTime.Now.DayOfWeek;
			return DateTime.Today.AddDays(dayOfWeek == 0 ? -6.0 : -dayOfWeek + 1);
		}

		private string Generate_WeekPlan(int startDay, int endDay = 7)
		{
			var stringBuilder = new StringBuilder();
			for (int index = startDay; index < endDay; ++index)
			{
				var day = FirstDayOfWeek().AddDays(index);
				stringBuilder.Append("<div class=\"sendeplan_day\">");
				stringBuilder.AppendFormat("<h4>{0}</h4>", day.ToString("dddd - dd.MM.yyyy", new CultureInfo("de-DE")));
				stringBuilder.Append(Generáte_Day(day));
				stringBuilder.Append("</div>");
			}
			return stringBuilder.ToString();
		}

		public string Handle_Get_Request(NamionoHttpContext request)
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("<section id=\"sendeplan\">");
			stringBuilder.Append(Generate_WeekPlan(0));
			stringBuilder.Append(Generate_WeekPlan(7, 14));
			stringBuilder.Append("</section>");
			return stringBuilder.ToString();
		}


		public void HeartBeat() => Database.HeartBeat();

		public void Install() => Provider.Install(nameof(Sendeplan), Members, Database, FileSystem);

		public void Remove(Guid id) => Members.Remove(id);

		public IMember Request(Guid id) => Members.ContainsKey(id) ? Members[id] : null;

		public void Start() => Database.Start();

		public void Stop() => Database.Start();

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
