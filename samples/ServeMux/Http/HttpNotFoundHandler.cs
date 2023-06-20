namespace ServeMux.Http;

public class HttpNotFoundHandler : IHttpHandler
{
    public string Route => "";

    public void ServeHttp(IHttpResponseWriter writer, IHttpRequest request)
    {
        writer.Write("404 not found");
    }
}
