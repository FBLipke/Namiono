namespace Namiono.Common.Provider
{
	public class ModuleLoadedEventArgs
	{
		public IProvider Module { get; private set; }

		public string Name { get; private set; }

		public ModuleLoadedEventArgs(string name, IProvider module)
		{
			Name = name.Substring(name.LastIndexOf('.') + 1);
			Module = module;
		}
	}
}
