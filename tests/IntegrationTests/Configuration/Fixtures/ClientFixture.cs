using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

namespace IntegrationTests.Configuration.Fixtures;

/// <summary>
/// Fixture for creating a containerized Client application instance.
/// <para>
/// This fixture builds and runs the Client application in a Docker container
/// using the project's source code and Dockerfile.
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// The container exposes the Client's HTTP endpoint on port 8080 and maps it to
/// a random host port for test access.
/// </para>
/// <para>
/// This fixture is primarily used in hybrid testing approaches where the Server
/// runs in-process and the Client runs in a container, or in end-to-end tests
/// where both Client and Server run in containers.
/// </para>
/// </remarks>
public sealed class ClientFixture() : ImageFixture(nameof(Client))
{
    /// <inheritdoc/>
    protected override IContainer BuildContainer(INetwork network) =>
        CreateRabbitMqConfiguredContainerBuilder(Image, network)
            .WithPortBinding(8080, assignRandomHostPort: true)
            .WithHostname(nameof(Client))
            .Build();

    /// <summary>
    /// Gets the hostname of the Client container.
    /// <para>
    /// This hostname can be used to connect to the Client container from other containers
    /// within the same Docker network.
    /// </para>
    /// </summary>
    public string? Hostname => Container.Hostname;

    /// <summary>
    /// Gets the public port mapped to the Client's HTTP port (8080).
    /// <para>
    /// This port can be used to make HTTP requests to the Client from the host machine.
    /// </para>
    /// </summary>
    public ushort? Port => Container.GetMappedPublicPort(8080);
}