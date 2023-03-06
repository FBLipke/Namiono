using Namiono.Common.Database;
using Namiono.Common.Network;
using System;
using System.Collections.Generic;

namespace Namiono.Common.Provider
{
	public interface IProvider : IManager
	{
		bool VolativeModule { get; set; }

		Dictionary<Guid, IMember> Members { get; set; }

		SqlDatabase Database { get; set; }

		FileSystem FileSystem { get; set; }

		bool CanEdit { get; set; }

		string FriendlyName { get; set; }

		string Description { get; set; }

		bool IsPublicModule { get; set; }

		bool CanAdd { get; set; }

		bool CanRemove { get; set; }

		void Remove(Guid id);

		bool Contains(Guid id);

		void Install();

		string Handle_Get_Request(NamionoHttpContext context);

		string Handle_Add_Request(NamionoHttpContext context);

		string Handle_Edit_Request(NamionoHttpContext context);

		string Handle_Remove_Request(NamionoHttpContext context);

		string Handle_Info_Request(NamionoHttpContext context);

		IMember Request(Guid id);

		bool Active { get; set; }
	}
}
