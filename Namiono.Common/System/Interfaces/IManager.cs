namespace Namiono.Common
{
	public interface IManager
	{
		void Start();

		void Stop();

		void HeartBeat();

		void Bootstrap();

		void Close();

		void Dispose();
	}
}
