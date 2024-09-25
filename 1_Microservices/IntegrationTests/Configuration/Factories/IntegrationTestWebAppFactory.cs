using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Images;
using IntegrationTests.Configuration.Fixtures;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace IntegrationTests.Configuration.Factories;

public abstract class IntegrationTestWebAppFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>, IAsyncLifetime
    where TEntryPoint : class
{
    private readonly RabbitMqFixture _rabbitMqFixture;

    protected IntegrationTestWebAppFactory()
    {
        TestcontainersSettings.ResourceReaperImage = new DockerImage("remote-docker-hub.artifactory.danskenet.net/testcontainers/ryuk:0.5.1");
        _rabbitMqFixture = new RabbitMqFixture();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddMassTransitTestHarness();
        });

        var inMemoryConfig = new Dictionary<string, string?>
        {
            ["RabbitMq:Host"] = "127.0.0.1",
            ["RabbitMq:Port"] = _rabbitMqFixture.Port.ToString(),
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


    public async Task InitializeAsync()
    {
        await _rabbitMqFixture.InitializeAsync();
    }

    public new async Task DisposeAsync()
    {
        await _rabbitMqFixture.DisposeAsync();
        await base.DisposeAsync();
    }
}