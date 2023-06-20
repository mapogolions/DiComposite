namespace ServeMux;

using System.Diagnostics;
using DiComposite;
using Microsoft.Extensions.DependencyInjection;
using ServeMux.Handlers;
using ServeMux.Http;

internal static class Program
{
    internal static void Main()
    {
        var services = new ServiceCollection();
        services.AddScoped<IHttpHandler, EchoHandler>();
        services.AddScoped<IHttpHandler, HttpNotFoundHandler>();
        services.AddScoped<IHttpHandler, GoodbyeHandler>();
        services.Composite<IHttpHandler, ServeMux>();

        var serviceProvider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true
        });

        using var httpServer = new HttpServer(serviceProvider);
        Debug.Assert("goodbye" == httpServer.Handle(new HttpRequest { Path = "/goodbye" }));
        Debug.Assert("foo" == httpServer.Handle(new HttpRequest { Path = "/echo", Body = "foo" }));
        Debug.Assert("404 not found" == httpServer.Handle(new HttpRequest { Path = "/unknown" }));
    }
}
