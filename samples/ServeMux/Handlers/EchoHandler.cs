namespace ServeMux.Handlers;

using ServeMux.Http;

public class EchoHandler : IHttpHandler
{
    public string Route => "echo";

    public void ServeHttp(IHttpResponseWriter writer, IHttpRequest request)
    {
        writer.Write(request.Body);
    }
}
