using Namiono.Common.Network;
using Namiono.Common.Provider;
using Namiono.Common.System;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Namiono.Common
{
	public class NamionoCommon : IManager, IDisposable
	{
		private Thread _heartBeatThread;

		public static NetworkManager NetworkManager { get; private set; }

		public static Dictionary<string, IProvider> Providers { get; private set; }

		public static Dictionary<string, IService> Services { get; private set; } = new Dictionary<string, IService>();

		public FileSystem Filesystem { get; }

		public bool Running { get; private set; }

		public FileSystem FileSystem { get; set; }

		public NamionoCommon()
		{
			Console.Title = "Namiono-Server";
			Console.WriteLine("Namiono-Server Version {0}", Assembly.GetExecutingAssembly().GetName().Version);
			Filesystem = new FileSystem(Environment.CurrentDirectory);
			Filesystem.CreateDirectory("Config");
			string directory = Filesystem.CreateDirectory(nameof(Services));
			if (Filesystem.Exists(directory))
			{
				var directories = new DirectoryInfo(directory).GetDirectories();
				if (directories.Length != 0)
					foreach (var directoryInfo in directories)
						Services.Add(directoryInfo.Name, new Service(directoryInfo.Name,
							directoryInfo.Name, Path.Combine(directory, directoryInfo.Name), "--daemon"));
			}
			_heartBeatThread = new Thread(new ThreadStart(HeartBeat));
			Providers = new Dictionary<string, IProvider>();
			Provider.Provider.ModuleLoaded += (sender, e) =>
			{
				Providers.Add(e.Name, e.Module);
				Log("I", "Common", string.Format("Loading Module \"{0}\"...", e.Module));

				Log("I", "Common", string.Format("Bootstrapping Module \"{0}\"...", e.Name));
				Provider.Provider.InvokeMethod<IProvider>(Providers[e.Name], "Bootstrap");

				Log("I", "Common", string.Format("Sending \"Install\" command  to \"{0}\"", e.Name));
				Provider.Provider.InvokeMethod<IProvider>(Providers[e.Name], "Install");

				Log("I", "Common", string.Format("Sending \"Start\" command  to \"{0}\"", e.Name));
				Provider.Provider.InvokeMethod<IProvider>(Providers[e.Name], "Start");
			};
			Task.Run(() =>
			{
				foreach (var file in new DirectoryInfo(Filesystem.Root)
					.GetFiles("*.Module.*.dll", SearchOption.TopDirectoryOnly))
					Provider.Provider.LoadModule(file.Name.Substring(0, file.Name.LastIndexOf('.')));
			});
			NetworkManager = new NetworkManager();
		}

		public void Start()
		{
			foreach (var manager in Services.Values)
				manager.Start();

			NetworkManager.Start();

			Running = Providers.Any();
			_heartBeatThread.Start();
		}

		public void Stop()
		{
			NetworkManager.Stop();
			foreach (var manager in Services.Values)
				manager.Stop();

			foreach (var provider in Providers)
			{
				Provider.Provider.InvokeMethod<IProvider>(provider.Value, "Stop");
				Log("I", provider.Key, "stopped!");
			}
		}

		public void HeartBeat()
		{
			while (Running)
			{
				Thread.Sleep(60000);
				NetworkManager.HeartBeat();

				foreach (var manager in Services.Values)
					manager.HeartBeat();

				foreach (var provider in Providers)
					Provider.Provider.InvokeMethod<IProvider>(provider.Value, "HeartBeat");
			}
		}

		public void Close()
		{
			NetworkManager.Close();

			foreach (var manager in Services.Values)
				manager.Close();

			foreach (var provider in Providers)
			{
				Provider.Provider.InvokeMethod<IProvider>(provider.Value, "Close");
				Log("I", provider.Key, "closed!");
			}
		}

		public void Dispose()
		{
			NetworkManager.Dispose();

			foreach (var manager in Services.Values)
				manager.Dispose();

			if (Providers.Any())
			{
				foreach (var provider in Providers)
					Provider.Provider.InvokeMethod<IProvider>(provider.Value, "Dispose");

				Providers.Clear();
				Providers = null;
			}

			try
			{
				_heartBeatThread.Abort();
			}
			catch
			{
			}

			_heartBeatThread = null;
		}

		public static void Log(string type, string name, string logmessage)
		{
			if (!Provider.Provider.CanDo("Log").Any())
				Console.WriteLine(logmessage);
			else
				Provider.Provider.InvokeMethod(Provider.Provider.CanDo("Log").First(), "Log",
					new object[] { type, name, logmessage });
				
		}

		public void Install()
		{

		}

		public void Bootstrap()
		{
			foreach (var manager in Services.Values)
			{
				manager.Bootstrap();
				Log("I", "Common", string.Format("Bootstrapping \"{0}\"...", manager.Name));
			}
		}
	}
}
