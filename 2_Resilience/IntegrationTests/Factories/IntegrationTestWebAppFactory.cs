using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Images;
using IntegrationTests.Fixtures;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace IntegrationTests.Factories;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly NetworkFixture _networkFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;

    public IntegrationTestWebAppFactory()
    {
        TestcontainersSettings.ResourceReaperImage = new DockerImage("remote-docker-hub.artifactory.danskenet.net/testcontainers/ryuk:0.5.1");
        
        _networkFixture = new NetworkFixture();
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
            ["RabbitMq:Port"] = _rabbitMqFixture.Port.ToString()
        };

        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddInMemoryCollection(inMemoryConfig);
        var configuration = configurationBuilder.Build();

        builder.UseConfiguration(configuration)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(inMemoryConfig);
            });
    }


    public async Task InitializeAsync()
    {
        await _networkFixture.InitializeAsync();
        await _rabbitMqFixture.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        await _rabbitMqFixture.DisposeAsync();
        await _networkFixture.DisposeAsync();
        await base.DisposeAsync();
    }
}