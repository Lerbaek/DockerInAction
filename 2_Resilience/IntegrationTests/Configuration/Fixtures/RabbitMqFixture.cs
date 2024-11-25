using DotNet.Testcontainers.Containers;
using IntegrationTests.Tests.EndToEnd;
using Testcontainers.RabbitMq;
using Xunit;

namespace IntegrationTests.Configuration.Fixtures;

/// <summary>
/// Create a RabbitMQ container with the management plugin enabled
/// </summary>
public sealed class RabbitMqFixture : ContainerFixture
{
    protected override IContainer Container => _rabbitMqContainer;

    public ushort Port => _rabbitMqContainer.GetMappedPublicPort(5672);

    public ushort ManagementPort => _rabbitMqContainer.GetMappedPublicPort(15672);

    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
        .WithName($"testcontainers-rabbitmq-{Guid.NewGuid()}")
        .WithImage("remote-docker-hub.artifactory.danskenet.net/rabbitmq:3.11.20-management")
        .WithHostname(nameof(RabbitMQ))
        .WithNetwork(Network)
        .WithPortBinding(15672, assignRandomHostPort: true)
        .WithUsername("guest")
        .WithPassword("guest")
        .Build();
}