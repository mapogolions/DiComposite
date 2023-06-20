using Microsoft.Extensions.DependencyInjection;

namespace DiComposite.Tests;

public class CompositeServiceCollectionExtensionsTests
{
    // [Theory]
    // [InlineData(false, false)]
    // [InlineData(false, true)]
    // [InlineData(true, false)]
    // [InlineData(true, true)]
    // public void ShouldBeAbleTo(bool validateScopes, bool validateOnBuild)
    // {
    //     // Arrange
    //     var services = new ServiceCollection();
    //     services.AddSingleton<IGreeting>(new HelloGreeting());
    //     services.AddScoped<IGreeting, WelcomeGreeting>();
    //     services.AddTransient<IGreeting>(sp => new HowdyGreeting());
    //     services.Composite<IGreeting, CompositeGreeting>();

    //     // Act
    //     using var sp = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = validateScopes, ValidateOnBuild = validateOnBuild });
    //     using var scope = sp.CreateScope();
    //     var greeting = scope.ServiceProvider.GetRequiredService<IGreeting>();
    //     var actual = greeting.Greet();

    //     // Assert
    //     Assert.Equal("hello,welcome,howdy", actual);
    // }

    [Fact]
    public void GetService_ShouldReturnComposite_WhenCalledOnRootScopeAndAllItemsHaveTransientLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<ISound, A>();
        services.AddTransient<ISound, B>();
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
    public void ShouldReturnCompositeWithTransientLifetime()
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
    public void ShouldReturnCompositeWithScopedLifetime()
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
    public void ShouldReturnCompositeWithSingletonLifetime()
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


    // [Fact]
    // public void Test2()
    // {
    //     // Arrange
    //     var services = new ServiceCollection();
    //     services.AddSingleton<IGreeting>(new HelloGreeting());
    //     services.AddScoped<IGreeting, WelcomeGreeting>();
    //     services.AddTransient<IGreeting>(sp => new HowdyGreeting());
    //     services.IComposite<IGreeting, CompositeGreeting>();

    //     // Act
    //     using var sp = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
    //     using var scope = sp.CreateScope();
    //     var greetingComposite = scope.ServiceProvider.GetRequiredService<IComposite<IGreeting>>();
    //     var actual = greetingComposite.Value.Greet();

    //     // Assert
    //     Assert.Equal("hello,welcome,howdy", actual);
    // }
}

internal interface ISound
{
    string Make();
}

internal class A : ISound
{
    public string Make() => "A";
}
internal class B : ISound
{
    public string Make() => "B";
}
internal class C : ISound
{
    public string Make() => "C";
}

internal class CompositeSound : ISound
{
    private readonly IEnumerable<ISound> _sounds;

    public CompositeSound(params ISound[] sounds)
    {
        _sounds = sounds;
    }

    public string Make() => string.Join("", _sounds.Select(x => x.Make()));
}
