using Microsoft.Extensions.DependencyInjection;

namespace ServeMux.Http;

public class HttpServer : IDisposable
{
    private readonly IServiceProvider _serviceProvider;

    public HttpServer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Dispose()
    {
        if (_serviceProvider is ServiceProvider sp)
        {
            sp.Dispose();
        }
    }

    public string Handle(IHttpRequest request)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var handler = scope.ServiceProvider.GetRequiredService<IHttpHandler>();
            var writer = new HttpResponseWriter();
            handler.ServeHttp(writer, request);
            return writer.Buffer.ToString();
        }
    }
}
