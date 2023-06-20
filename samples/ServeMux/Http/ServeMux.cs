namespace ServeMux.Http;

public class ServeMux : IHttpHandler
{
    private readonly Dictionary<string, IHttpHandler> _handlers;

    public string Route => "*";

    public ServeMux(params IHttpHandler[] handlers)
    {
        _handlers = handlers.ToDictionary(x => x.Route);
    }

    public void ServeHttp(IHttpResponseWriter writer, IHttpRequest request)
    {
        string path = request.Path.Trim(new[] { '/' });
        IHttpHandler? handler = null;
        if (_handlers.TryGetValue(path, out handler))
        {
            handler.ServeHttp(writer, request);
            return;
        }
        handler = _handlers.FirstOrDefault(x => x.Value is HttpNotFoundHandler).Value;
        if (handler is null)
        {
            throw new NotImplementedException();
        }
        handler.ServeHttp(writer, request);
    }
}
