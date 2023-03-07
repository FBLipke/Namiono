namespace Namiono.Common
{
	public interface IService : IManager
	{
		bool Enabled { get; set; }

		bool IsLinux { get; set; }

		string Name { get; set; }

		string Arguments { get; set; }

		string FileName { get; set; }

		string WorkingDir { get; set; }

		bool IsRunning { get; set; }
	}
}
