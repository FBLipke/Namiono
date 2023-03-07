using System;
using System.Collections.Generic;

namespace Namiono.Common.Provider
{
	public interface IMember
	{
		Dictionary<Guid, IMember> Members { get; set; }

		double Created { get; set; }

		string Url { get; set; }

		double Updated { get; set; }

		string OutPut { get; set; }

		Guid Author { get; set; }

		int FormatType { get; set; }

		int ControlType { get; set; }

		string Frame { get; set; }

		string Provider { get; set; }

		string Design { get; set; }

		string Image { get; set; }

		int Width { get; set; }

		int Height { get; set; }

		string Name { get; set; }

		string Password { get; set; }

		bool Active { get; set; }

		string Action { get; set; }

		string Description { get; set; }

		ulong Level { get; set; }

		Guid Access { get; set; }

		bool Locked { get; set; }

		bool Service { get; set; }

		bool Moderator { get; set; }

		string Target { get; set; }

		ushort Port { get; set; }

		string Salt { get; set; }

		string IpAddress { get; set; }

		Guid Id { get; set; }

		string EMail { get; set; }

		string ExtraData { get; set; }
	}
}
