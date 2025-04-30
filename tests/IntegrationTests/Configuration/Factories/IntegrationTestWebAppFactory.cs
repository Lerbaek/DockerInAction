using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Images;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace IntegrationTests.Configuration.Factories;

public abstract class IntegrationTestWebAppFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    private bool _useTestHarness;

    public RabbitMqTransportOptions RabbitMqOptions { get; } = new();

    public void UseTestHarness(bool use = true) => _useTestHarness = use;

    protected IntegrationTestWebAppFactory()
    {
        TestcontainersSettings.ResourceReaperImage = new DockerImage("remote-docker-hub.artifactory.danskenet.net/testcontainers/ryuk:0.11.0");
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
}