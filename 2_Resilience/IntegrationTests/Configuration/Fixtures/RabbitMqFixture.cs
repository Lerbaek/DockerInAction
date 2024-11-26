using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using IntegrationTests.Configuration.Factories;
using Testcontainers.RabbitMq;
using Xunit;

namespace IntegrationTests.Configuration.Fixtures;

/// <summary>
/// Create a RabbitMQ container with the management plugin enabled
/// </summary>
public sealed class RabbitMqFixture<T> : ContainerFixture<T>
{
    private RabbitMqContainer? _rabbitMqContainer;

    protected override IContainer Container => _rabbitMqContainer ??= new RabbitMqBuilder()
            .WithName($"testcontainers-rabbitmq-{Guid.NewGuid()}")
            .WithImage("remote-docker-hub.artifactory.danskenet.net/rabbitmq:3.11.20-management")
            .WithHostname(nameof(RabbitMQ))
            .WithPortBinding(15672, assignRandomHostPort: true)
            .WithUsername("guest")
            .WithPassword("guest")
            .WithNetwork(Network)
            .Build();

    public string Hostname => Container.Hostname;

    public ushort Port => Container.GetMappedPublicPort(5672);

    public ushort ManagementPort => Container.GetMappedPublicPort(15672);
}