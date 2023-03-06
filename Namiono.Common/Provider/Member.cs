using System;
using System.Collections.Generic;

namespace Namiono.Common.Provider
{
	public class Member : IMember
	{
		public double Created { get; set; } = 0.0;

		public double Updated { get; set; } = 0.0;

		public string Url { get; set; } = "-";

		public string OutPut { get; set; } = "-";

		public Guid Author { get; set; } = Guid.Empty;

		public int FormatType { get; set; } = 0;

		public int ControlType { get; set; } = 0;

		public string Frame { get; set; } = "-";

		public string Provider { get; set; } = "-";

		public string Design { get; set; } = "-";

		public string Image { get; set; } = "-";

		public int Width { get; set; } = 0;

		public int Height { get; set; } = 0;

		public string Name { get; set; } = "-";

		public string Password { get; set; } = "-";

		public bool Active { get; set; } = false;

		public string Action { get; set; } = "-";

		public string Description { get; set; } = "-";

		public ulong Level { get; set; } = 0;

		public Guid Access { get; set; } = Guid.Empty;

		public bool Locked { get; set; } = false;

		public bool Service { get; set; } = false;

		public bool Moderator { get; set; } = false;

		public string Target { get; set; } = "-";

		public ushort Port { get; set; } = 0;

		public string IpAddress { get; set; } = "0.0.0.0";

		public Guid Id { get; set; } = Guid.Empty;

		public string ExtraData { get; set; } = "-";

		public string EMail { get; set; } = "-";

		public Dictionary<Guid, IMember> Members { get; set; }

		public string Salt { get; set; } = "";
	}
}
