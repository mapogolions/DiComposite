using Microsoft.Extensions.DependencyInjection;

namespace ServeMux.Http;

public class HttpServer
{
    private readonly IServiceScopeFactory _scopeFactory;

    public HttpServer(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
    }

    public string Handle(IHttpRequest request)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var handler = scope.ServiceProvider.GetRequiredService<IHttpHandler>();
            var writer = new HttpResponseWriter();
            handler.ServeHttp(writer, request);
            return writer.Buffer.ToString();
        }
    }
}
