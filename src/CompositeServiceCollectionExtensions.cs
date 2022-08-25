using Microsoft.Extensions.DependencyInjection;

namespace DiComposite;

public static class CompositeServiceCollectionExtensions
{
    public static IServiceCollection Composite<TService, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime)
        where TImplementation : class, TService
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        var serviceType = typeof(TService);
        var indexedDescriptors = services.WithIndex().Where(x => x.value.ServiceType == serviceType).ToList();
        if (!indexedDescriptors.Any())
        {
            throw new ArgumentException($"Service type {serviceType.FullName} not found");
        }
        foreach (var (index, descriptor) in indexedDescriptors)
        {
            if (descriptor.ImplementationInstance is not null)
            {
                services[index] = new ServiceDescriptor(descriptor.ImplementationInstance.GetType(), descriptor.ImplementationInstance);
                continue;
            }
            if (descriptor.ImplementationType is not null)
            {
                services[index] = new ServiceDescriptor(descriptor.ImplementationType, descriptor.ImplementationType, descriptor.Lifetime);
            }
        }
        var composite = new ServiceDescriptor(serviceType, sp =>
        {
            var instances = indexedDescriptors
                .Select<(int index, ServiceDescriptor value), TService>(x =>
                {
                    var descriptor = x.value;
                    if (descriptor.ImplementationFactory is not null)
                    {
                        services.RemoveAt(x.index);
                        return (TService)descriptor.ImplementationFactory(sp);
                    }
                    var concreteType = descriptor.ImplementationInstance is not null ? descriptor.ImplementationInstance.GetType() : descriptor.ImplementationType;
                    if (concreteType is null) throw new InvalidOperationException();
                    return (TService)sp.GetRequiredService(concreteType);
                });
            return ActivatorUtilities.CreateInstance<TImplementation>(sp, instances.ToArray());
        }, lifetime);
        services.Add(composite);
        return services;
    }
}

public static class EnumerableExtensions
{
    public static IEnumerable<(int index, T value)> WithIndex<T>(this IEnumerable<T> source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        int index = 0;
        foreach (var value in source) yield return (index++, value);
    }
}
