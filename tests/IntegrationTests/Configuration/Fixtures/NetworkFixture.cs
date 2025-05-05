using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using Xunit;

namespace IntegrationTests.Configuration.Fixtures;

/// <summary>
/// Fixture for creating a Docker network used to connect containers during tests.
/// <para>
/// This fixture creates a uniquely named Docker network that allows containers to
/// communicate with each other using container names as hostnames.
/// </para>
/// </summary>
/// <remarks>
/// The network is created with a unique name to avoid conflicts between test runs.
/// All containers that need to communicate with each other should be connected to this network.
/// </remarks>
public class NetworkFixture : IAsyncLifetime
{
    /// <summary>
    /// Gets the Docker network instance.
    /// <para>
    /// This network can be used to connect multiple containers so they can communicate
    /// using container names as hostnames.
    /// </para>
    /// </summary>
    public INetwork Network { get; } = new NetworkBuilder()
        .WithName($"testcontainers-microservices-{Guid.NewGuid()}")
        //.WithCleanUp(true)
        .Build();

    /// <summary>
    /// Initializes the Docker network by creating it.
    /// </summary>
    /// <returns>A task representing the asynchronous initialization operation.</returns>
    public async Task InitializeAsync()
    {
        await Network.CreateAsync();
    }

    /// <summary>
    /// Disposes the Docker network, removing it from Docker.
    /// </summary>
    /// <returns>A task representing the asynchronous disposal operation.</returns>
    public async Task DisposeAsync()
    {
        await Network.DisposeAsync();
    }
}