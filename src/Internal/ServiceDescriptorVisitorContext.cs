using Microsoft.Extensions.DependencyInjection;

namespace DiComposite.Internal;

internal class ServiceDescriptorVisitorContext
{
    public IServiceCollection? Services { get; init; }
}
