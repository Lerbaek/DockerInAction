using DotNet.Testcontainers.Networks;
using Testcontainers.RabbitMq;
using IContainer = DotNet.Testcontainers.Containers.IContainer;

namespace IntegrationTests.Configuration.Fixtures;

/// <summary>
/// Fixture for creating a RabbitMQ container with the management plugin enabled.
/// <para>
/// This fixture provides a containerized RabbitMQ instance for testing message-based integrations
/// between applications. The container has the management plugin enabled for easier debugging.
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// The container exposes both the standard RabbitMQ port (5672) and the management port (15672).
/// </para>
/// <para>
/// The container is configured with default credentials (guest/guest).
/// </para>
/// </remarks>
public class RabbitMqFixture : ContainerFixture
{
    /// <inheritdoc/>
    protected override IContainer BuildContainer(INetwork network) => new RabbitMqBuilder()
            .WithName($"testcontainers-rabbitmq-{Guid.NewGuid()}")
            .WithImage("rabbitmq:3.11.20-management")
            .WithHostname(nameof(RabbitMQ))
            .WithPortBinding(15672, assignRandomHostPort: true)
            .WithUsername("guest")
            .WithPassword("guest")
            .WithNetwork(network)
            .Build();

    /// <summary>
    /// Gets the hostname of the RabbitMQ container.
    /// <para>
    /// This hostname can be used to connect to the RabbitMQ container from other containers
    /// within the same network.
    /// </para>
    /// </summary>
    public string Hostname => Container.Hostname;

    /// <summary>
    /// Gets the public port mapped to the RabbitMQ AMQP protocol port (5672).
    /// <para>
    /// This port can be used to connect to RabbitMQ from the host machine.
    /// </para>
    /// </summary>
    public ushort Port => Container.GetMappedPublicPort(5672);

    /// <summary>
    /// Gets the public port mapped to the RabbitMQ management interface port (15672).
    /// <para>
    /// This port can be used to access the RabbitMQ management web interface from a browser.
    /// </para>
    /// </summary>
    public ushort ManagementPort => Container.GetMappedPublicPort(15672);
}