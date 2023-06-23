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
        services.Composite<IHttpHandler, ServeMux>();
        services.AddSingleton<HttpServer>();

        using var serviceProvider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true
        });

        var httpServer = serviceProvider.GetRequiredService<HttpServer>();
        Debug.Assert("foo" == httpServer.Handle(new HttpRequest { Path = "/echo", Body = "foo" }));
        Debug.Assert("404 not found" == httpServer.Handle(new HttpRequest { Path = "/unknown" }));
    }
}
