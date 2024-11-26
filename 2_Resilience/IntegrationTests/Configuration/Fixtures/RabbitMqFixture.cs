using System.ComponentModel;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using IntegrationTests.Configuration.Factories;
using Testcontainers.RabbitMq;
using Xunit;
using IContainer = DotNet.Testcontainers.Containers.IContainer;

namespace IntegrationTests.Configuration.Fixtures;

/// <summary>
/// Create a RabbitMQ container with the management plugin enabled
/// </summary>
public class RabbitMqFixture : ContainerFixture
{
    protected override IContainer BuildContainer(INetwork network) => new RabbitMqBuilder()
            .WithName($"testcontainers-rabbitmq-{Guid.NewGuid()}")
            .WithImage("remote-docker-hub.artifactory.danskenet.net/rabbitmq:3.11.20-management")
            .WithHostname(nameof(RabbitMQ))
            .WithPortBinding(15672, assignRandomHostPort: true)
            .WithUsername("guest")
            .WithPassword("guest")
            .WithNetwork(network)
            .Build();

    public string Hostname => Container.Hostname;

    public ushort Port => Container.GetMappedPublicPort(5672);

    public ushort ManagementPort => Container.GetMappedPublicPort(15672);
}