namespace ServeMux.Handlers;

using ServeMux.Http;

public class GoodbyeHandler : IHttpHandler
{
    public string Route => "goodbye";

    public void ServeHttp(IHttpResponseWriter writer, IHttpRequest request)
    {
        writer.Write("goodbye");
    }
}
