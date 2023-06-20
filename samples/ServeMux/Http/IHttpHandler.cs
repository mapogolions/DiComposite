namespace ServeMux.Http;

public interface IHttpHandler
{
    string Route { get; }
    void ServeHttp(IHttpResponseWriter writer, IHttpRequest request);
}
