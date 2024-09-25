using Testcontainers.RabbitMq;
using Xunit;

namespace IntegrationTests.Configuration.Fixtures;

public sealed class RabbitMqFixture : IAsyncLifetime
{
    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
        .WithName($"testcontainers-rabbitmq-{Guid.NewGuid()}")
        .WithImage("remote-docker-hub.artifactory.danskenet.net/rabbitmq:3.11.20-management")
        .Build();

    public async Task InitializeAsync()
    {
        await _rabbitMqContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _rabbitMqContainer.StopAsync();
    }

    public int Port => _rabbitMqContainer.GetMappedPublicPort(5672);
}