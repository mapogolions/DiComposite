### DiComposite

Composite pattern for Microsoft.Extensions.DependencyInjection

#### How to use

- using the `IServiceCollection.Composite()` method

```c#
interface IHofService {}
class HofService
{
    public HofService(IService service) {}
}

class CompositeService : IService
{
    public CompositeService(params IService[] services) {}
}

var services = new ServiceCollection();
services.AddSingleton<IService>(new ServiceA());
services.AddScoped<IService, ServiceB>();
services.AddTransient<IService>(sp => new ServiceC());
services.Composite<IService, CompositeService>();
services.AddScoped<IHofService, HofService>();
```


- using the `IComposite` interface and the `IServiceCollection.IComposite()` method

```c#
interface IHofService {}

class HofService
{
    public HofService(IComposite<IService> composite) {}
}

class CompositeService : IService
{
    public CompositeService(params IService[] services) {}
}
var services = new ServiceCollection();
services.AddSingleton<IService>(new ServiceA());
services.AddScoped<IService, ServiceB>();
services.AddTransient<IService>(sp => new ServiceC());
services.IComposite<ISound, CompositeSound>();
services.AddScoped<IHofService, HofService>();
```
