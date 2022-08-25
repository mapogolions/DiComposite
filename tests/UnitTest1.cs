using Microsoft.Extensions.DependencyInjection;

namespace DiComposite.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IGreeting>(new HelloGreeting());
        services.AddScoped<IGreeting, WelcomeGreeting>();
        services.AddTransient<IGreeting>(sp => new HowdyGreeting());
        services.Composite<IGreeting, CompositeGreeting>(ServiceLifetime.Singleton);
        using var sp = services.BuildServiceProvider();
        var greeting = sp.GetRequiredService<IGreeting>();
        var message = greeting.Greet();
        Assert.Equal("hello,welcome,howdy", message);
    }

    public interface IGreeting
    {
        string Greet();
    }

    public class HelloGreeting : IGreeting
    {
        public string Greet() => "hello";
    }
    public class WelcomeGreeting : IGreeting
    {
        public string Greet() => "welcome";
    }
    public class HowdyGreeting : IGreeting
    {
        public string Greet() => "howdy";
    }

    public class CompositeGreeting : IGreeting
    {
        private readonly IEnumerable<IGreeting> _greetings;

        public CompositeGreeting(params IGreeting[] greetings)
        {
            _greetings = greetings;
        }

        public string Greet() => string.Join(',', _greetings.Select(x => x.Greet()));
    }
}
