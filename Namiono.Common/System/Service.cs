using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Namiono.Common.System
{
	public class Service : IDisposable, IService, IManager
	{
		private Process prc;

		private FileSystem FileSystem { get; set; }

		public bool Enabled { get; set; } = false;

		public Service(string name, string filename, string workingDir, string args)
		{
			WorkingDir = workingDir;
			FileSystem = new FileSystem(WorkingDir);
			Name = name;
			FileName = Path.Combine(Path.Combine(FileSystem.Root, "bin"), IsLinux ? filename : string.Format("{0}.exe", filename));
			Arguments = args;
		}

		public void Bootstrap()
		{
			Console.WriteLine("[I] Init Service: {0}", Name);
			if (!Enabled)
			{
				Console.WriteLine("[I] {0} is Disabled!:", Name);
			}
			else
			{
				if (!FileSystem.Exists(WorkingDir))
				{
					string[] strArray = new string[4]
					{
			"logs",
			"config",
			"tmp",
			"bin"
					};
					foreach (string path2 in strArray)
					{
						string path = Path.Combine(FileSystem.CreateDirectory(WorkingDir), path2);
						Console.WriteLine("[I] Creating Directory {0}...", path);
						FileSystem.CreateDirectory(path);
					}
				}
				prc = new Process();
				prc.StartInfo.FileName = FileName;
				prc.StartInfo.Arguments = Arguments;
				prc.StartInfo.CreateNoWindow = true;
				prc.StartInfo.UseShellExecute = false;
				prc.StartInfo.WorkingDirectory = WorkingDir;
				prc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			}
		}

		public void Start()
		{
			if (!Enabled)
				return;
			foreach (Process process in Process.GetProcessesByName(Name))
			{
				NamionoCommon.Log("I", Name, "Destroying old Process!");
				process.Kill();
				Thread.Sleep(1);
			}
			NamionoCommon.Log("I", Name, "Starting Service: (with Arguments: {Arguments})");
			IsRunning = prc.Start();
			if (IsRunning)
				NamionoCommon.Log("I", Name, "Up and running!");
		}

		public void Stop()
		{
			Console.WriteLine("[I] Closing Process: {0}", Name);
			if (!IsRunning)
				return;
			prc.Kill();
		}

		public void HeartBeat()
		{
			if (IsRunning)
				return;
			Console.WriteLine("Status: Service \"{0}\" is not running (anymore)!", Name);
			Stop();
		}

		public bool IsLinux { get; set; } = Environment.OSVersion.Platform == PlatformID.Unix;

		public string Name { get; set; }

		public string Arguments { get; set; }

		public string FileName { get; set; }

		public string WorkingDir { get; set; }

		public bool IsRunning { get; set; }

		public void Dispose() => prc.Dispose();

		public void Close() => Stop();
	}
}
