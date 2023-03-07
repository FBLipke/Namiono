namespace Namiono.Common.Network
{
	public class NamionoHttpContext
	{

		public HttpRequest Request { get; private set; }

		public HttpResponse Response { get; private set; }

		public NamionoHttpContext(HttpRequest request)
		{
			Request = request;
		}
	}
}
