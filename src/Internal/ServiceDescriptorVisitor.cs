using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace DiComposite.Internal;

internal class ServiceDescriptorVisitor
{
    public void VisitDescriptor(ServiceDescriptor descriptor, ServiceDescriptorVisitorContext context)
    {
        if (descriptor.ImplementationInstance is not null)
        {
            VisitInstance(descriptor, context);
            return;
        }
        if (descriptor.ImplementationType is not null)
        {
            VisitImplementationType(descriptor, context);
            return;
        }
        if (descriptor.ImplementationFactory is not null)
        {
            VisitFactory(descriptor, context);
            return;
        }
        throw new InvalidOperationException();

    }

    private void VisitInstance(ServiceDescriptor descriptor, ServiceDescriptorVisitorContext context)
    {
        Debug.Assert(descriptor.ImplementationInstance is not null);
        context.Services.Remove(descriptor);
        var implementationType = descriptor.ImplementationInstance.GetType();
        context.Services.Add(new ServiceDescriptor(implementationType, descriptor.ImplementationInstance));
    }

    private void VisitImplementationType(ServiceDescriptor descriptor, ServiceDescriptorVisitorContext context)
    {
        Debug.Assert(descriptor.ImplementationType is not null);
        context.Services.Remove(descriptor);
        context.Services.Add(
            new ServiceDescriptor(descriptor.ImplementationType, descriptor.ImplementationType,descriptor.Lifetime));
    }

    private void VisitFactory(ServiceDescriptor descriptor, ServiceDescriptorVisitorContext context)
    {
        Debug.Assert(descriptor.ImplementationFactory is not null);
        context.Services.Remove(descriptor);
    }
}
