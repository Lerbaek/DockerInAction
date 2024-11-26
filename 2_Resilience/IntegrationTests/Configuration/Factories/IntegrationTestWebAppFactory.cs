using Client.Controllers;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using IntegrationTests.Configuration.Fixtures;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests.Configuration.Factories;

public class NetworkFixture<T> : IAsyncLifetime
{
    public NetworkFixture()
    {
        Instance = this;
    }

    public static NetworkFixture<T> Instance { get; private set; } = new();

    public INetwork Network { get; } = CreateNetwork(typeof(T).Name);

    public static INetwork CreateNetwork(string identifier) => new NetworkBuilder()
        .WithName($"testcontainers-microservices-{Guid.NewGuid()}")
        .WithCleanUp(true)
        .Build();

    public Task InitializeAsync() => Task.CompletedTask;//await Network.CreateAsync();

    public Task DisposeAsync() => Task.CompletedTask;
}

public abstract class IntegrationTestWebAppFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    private bool _useTestHarness;

    public RabbitMqTransportOptions RabbitMqOptions { get; } = new();

    public void UseTestHarness(bool use = true) => _useTestHarness = use;

    protected IntegrationTestWebAppFactory()
    {
        TestcontainersSettings.ResourceReaperImage = new DockerImage("remote-docker-hub.artifactory.danskenet.net/testcontainers/ryuk:0.5.1");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
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

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
    }
}