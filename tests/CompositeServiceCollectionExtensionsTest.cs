using DiComposite.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;

namespace DiComposite.Tests;

public class CompositeServiceCollectionExtensionsTests
{
    [Fact]
    public void ShouldRegisterCompositeAsTransientService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ISound>(new A());
        services.AddScoped<ISound, B>();
        services.AddTransient<ISound>(sp => new C());
        services.Composite<ISound, CompositeSound>();

        // Act
        var descriptor = services.Single(x => x.ServiceType == typeof(ISound));

        // Assert
        Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
    }

    [Fact]
    public void ShouldRegisterCompositeAsScopedService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<ISound, B>();
        services.AddSingleton<ISound>(new A());
        services.Composite<ISound, CompositeSound>();

        // Act
        var descriptor = services.Single(x => x.ServiceType == typeof(ISound));

        // Assert
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [Fact]
    public void ShouldRegisterCompositeAsSingletonService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ISound, B>();
        services.AddSingleton<ISound>(new A());
        services.Composite<ISound, CompositeSound>();

        // Act
        var descriptor = services.Single(x => x.ServiceType == typeof(ISound));

        // Assert
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void CompositeShouldBeAbleToConsumeOtherServicesFromContainer()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<ISound, A>();
        services.AddTransient<ISound, B>();
        services.AddTransient<ISound>(sp => new C());
        services.AddSingleton<IPronunciation, PronunciationWithPause>();
        services.Composite<ISound, CompositeSoundWithDep>();

        // Act
        using var sp = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
        var sound = sp.GetRequiredService<ISound>();

        // Assert
        Assert.Equal(" A B C", sound.Make());
    }

    [Fact]
    public void GetService_ShouldBeAbleToResolveTransientCompositeService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ISound>(new A());
        services.AddSingleton<ISound, B>();
        services.AddTransient<ISound>(sp => new C());
        services.Composite<ISound, CompositeSound>();

        // Act
        using var sp = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
        var sound = sp.GetRequiredService<ISound>();

        // Assert
        Assert.Equal("ABC", sound.Make());
        Assert.NotSame(sound, sp.GetRequiredService<ISound>());
    }

    [Fact]
    public void GetService_ShouldBeAleToResolveScopedCompositeService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<ISound, B>();
        services.AddSingleton<ISound>(new A());
        services.Composite<ISound, CompositeSound>();

        // Act + Assert
        using var sp = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
        ISound? sound1 = null;
        using (var scope = sp.CreateScope())
        {
            sound1 = scope.ServiceProvider.GetRequiredService<ISound>();
            Assert.Equal("BA", sound1.Make());
            Assert.Same(sound1, scope.ServiceProvider.GetRequiredService<ISound>());
        }
        using (var scope = sp.CreateScope())
        {
            Assert.NotSame(sound1, scope.ServiceProvider.GetRequiredService<ISound>());
        }
    }

    [Fact]
    public void Test2()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ISound>(new A());
        services.AddScoped<ISound, B>();
        services.AddTransient<ISound>(sp => new C());
        services.IComposite<ISound, CompositeSound>();

        // Act
        using var sp = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
        using var scope = sp.CreateScope();
        var greetingComposite = scope.ServiceProvider.GetRequiredService<IComposite<ISound>>();
        var actual = greetingComposite.Value.Make();

        // Assert
        Assert.Equal("ABC", actual);
    }
}
