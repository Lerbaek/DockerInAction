using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Images;
using IntegrationTests.Configuration.Fixtures;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace IntegrationTests.Configuration.Factories;

/// <summary>
/// Base class for web application factories used in integration testing.
/// <para>
/// This class extends WebApplicationFactory to enable integration testing with containerized 
/// dependencies. It provides:
/// </para>
/// <list type="bullet">
/// <item>Substituted logging for capturing and verifying log output</item>
/// <item>Configuration of RabbitMQ connection settings for messaging tests</item>
/// <item>Optional MassTransit test harness integration</item>
/// </list>
/// </summary>
/// <typeparam name="TEntryPoint">The entry point class of the application being tested.</typeparam>
public abstract class IntegrationTestWebAppFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>, IIntegrationTestWebAppFactory
    where TEntryPoint : class
{
    private bool _useTestHarness;

    /// <summary>
    /// Gets the substituted logger that can be used to verify logging behavior.
    /// <para>
    /// This logger is injected into the application to capture log messages during test execution.
    /// Tests can then verify specific log messages or patterns.
    /// </para>
    /// </summary>
    public ILogger Logger { get; } = Substitute.For<ILogger>();
    
    /// <summary>
    /// Registers an action to be executed when a log message matching the specified condition is logged.
    /// <para>
    /// This is useful for detecting when specific log messages occur during test execution,
    /// allowing tests to synchronize with asynchronous operations.
    /// </para>
    /// </summary>
    /// <param name="condition">A function that returns true when a log message matches the desired criteria.</param>
    /// <param name="action">The action to execute when the condition is met.</param>
    public void SetLogAction(Func<string, bool> condition, Action action)
    {
        Logger.When(logger => logger
                .Log(
                    Arg.Any<LogLevel>(),
                    Arg.Any<EventId>(),
                    Arg.Any<Arg.AnyType>(),
                    Arg.Any<Exception>(),
                    Arg.Any<Func<Arg.AnyType, Exception?, string>>()))
            .Do(info =>
            {
                var logMsg = info.ArgAt<object>(2).ToString() ?? string.Empty;
                if (condition(logMsg))
                {
                    action();
                }
            });
    }

    /// <summary>
    /// Gets the RabbitMQ transport options used to configure the application.
    /// <para>
    /// These options are set in the application configuration to connect to
    /// the containerized RabbitMQ instance.
    /// </para>
    /// </summary>
    public RabbitMqTransportOptions RabbitMqOptions { get; } = new();

    /// <summary>
    /// Configures whether to use MassTransit's test harness for testing message consumers.
    /// <para>
    /// When enabled, the test harness replaces the actual message broker with an in-memory
    /// implementation, allowing tests to directly observe and verify message handling.
    /// </para>
    /// </summary>
    /// <param name="use">True to use the test harness, false otherwise.</param>
    public void UseTestHarness(bool use = true) => _useTestHarness = use;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegrationTestWebAppFactory{TEntryPoint}"/> class.
    /// <para>
    /// Sets up the Testcontainers resource reaper image to use the corporate Docker registry.
    /// </para>
    /// </summary>
    protected IntegrationTestWebAppFactory()
    {
        TestcontainersSettings.ResourceReaperImage = new DockerImage("remote-docker-hub.artifactory.danskenet.net/testcontainers/ryuk:0.11.0");
    }

    /// <inheritdoc/>
    public void ConfigureRabbitMq(RabbitMqFixture rabbitMqFixture)
    {
        RabbitMqOptions.Host = rabbitMqFixture.Hostname;
        RabbitMqOptions.Port = rabbitMqFixture.Port;
    }

    /// <summary>
    /// Configures the web host for integration testing.
    /// <para>
    /// This method sets up:
    /// </para>
    /// <list type="bullet">
    /// <item>Substituted logging to capture and verify application logs</item>
    /// <item>MassTransit test harness (when enabled)</item>
    /// <item>RabbitMQ connection configuration</item>
    /// </list>
    /// </summary>
    /// <param name="builder">The web host builder to configure.</param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var loggerFactory = Substitute.For<ILoggerFactory>();
        loggerFactory.CreateLogger(Arg.Any<string>()).Returns(Logger);
        builder.ConfigureServices(services => services.AddSingleton(loggerFactory));

        if (_useTestHarness)
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddMassTransitTestHarness();
            });
        }

        var inMemoryConfig = new Dictionary<string, string?>
        {
            ["RabbitMq:Host"] = RabbitMqOptions.Host,
            ["RabbitMq:Port"] = RabbitMqOptions.Port.ToString(),
            ["RabbitMq:Retries"] = "2",
        };

        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddInMemoryCollection(inMemoryConfig);
        var configuration = configurationBuilder.Build();

        builder.UseConfiguration(configuration)
            .ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(inMemoryConfig);
            });
    }
}