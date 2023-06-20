using DiComposite.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace DiComposite;

public static class CompositeServiceCollectionExtensions
{
    private static readonly ServiceDescriptorVisitor _visitor = new();

    public static IServiceCollection Composite<TService, TImplementation>(this IServiceCollection services)
        where TImplementation : class, TService
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        var descriptors = services.Where(x => x.ServiceType == typeof(TService)).ToList();
        if (!descriptors.Any())
        {
            throw new ArgumentException($"Service type {typeof(TService).FullName} not found");
        }
        var context = new ServiceDescriptorVisitorContext { Services = services };
        descriptors.ForEach(x => _visitor.VisitDescriptor(x, context));
        var composite = CompositeFactory<TService, TImplementation>(descriptors);
        services.Add(composite);
        return services;
    }

    public static IServiceCollection IComposite<TService, TImplementation>(this IServiceCollection services)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        var serviceType = typeof(IComposite<>).MakeGenericType(typeof(TService));
        services.AddTransient(serviceType, sp =>
        {
            var instances = sp.GetRequiredService<IEnumerable<TService>>();
            TImplementation instance = (TImplementation)Activator.CreateInstance(typeof(TImplementation), instances)!;
            return (IComposite<TService>)Activator.CreateInstance(typeof(Composite<TService>), new object[] { instance })!;
        });
        return services;
    }

    private static ServiceDescriptor CompositeFactory<TService, TImplementation>(IReadOnlyList<ServiceDescriptor> descriptors)
    {
        var lifetime = descriptors.Max(x => x.Lifetime);
        return new  ServiceDescriptor(typeof(TService), sp =>
        {
            var instances = descriptors.Select(x =>
            {
                var descriptor = x;
                if (descriptor.ImplementationFactory is not null)
                {
                    return (TService)descriptor.ImplementationFactory(sp);
                }
                var concreteType = descriptor.ImplementationInstance is not null ? descriptor.ImplementationInstance.GetType() : descriptor.ImplementationType;
                if (concreteType is null) throw new InvalidOperationException();
                return (TService)sp.GetRequiredService(concreteType);
            }).ToArray();
            return Activator.CreateInstance(typeof(TImplementation),instances)!;
        },
        lifetime);
    }
}
