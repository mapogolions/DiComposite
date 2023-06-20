using Microsoft.Extensions.DependencyInjection;

namespace DiComposite.Tests;

public partial class CompositeServiceCollectionExtensionsTests
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void Test1(bool validateScopes, bool validateOnBuild)
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IGreeting>(new HelloGreeting());
        services.AddScoped<IGreeting, WelcomeGreeting>();
        services.AddTransient<IGreeting>(sp => new HowdyGreeting());
        services.Composite<IGreeting, CompositeGreeting>(ServiceLifetime.Transient);

        // Act
        using var sp = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = validateScopes, ValidateOnBuild = validateOnBuild });
        using var scope = sp.CreateScope();
        var greeting = scope.ServiceProvider.GetRequiredService<IGreeting>();
        var actual = greeting.Greet();

        // Assert
        Assert.Equal("hello,welcome,howdy", actual);
    }


    internal class HelloGreeting : IGreeting
    {
        public string Greet() => "hello";
    }
    internal class WelcomeGreeting : IGreeting
    {
        public string Greet() => "welcome";
    }
    internal class HowdyGreeting : IGreeting
    {
        public string Greet() => "howdy";
    }

    internal class CompositeGreeting : IGreeting
    {
        private readonly IEnumerable<IGreeting> _greetings;

        public CompositeGreeting(params IGreeting[] greetings)
        {
            _greetings = greetings;
        }

        public string Greet() => string.Join(',', _greetings.Select(x => x.Greet()));
    }
}