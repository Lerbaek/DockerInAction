using IntegrationTests.Configuration.Fixtures;
using IntegrationTests.Tests.Testcontainers.EndToEnd;

namespace IntegrationTests.Tests.Testcontainers.Fixtures;

/// <summary>
/// Test fixtures for end-to-end integration tests using Testcontainers.
/// <para>
/// This fixture combines all required container components for running fully containerized end-to-end tests:
/// </para>
/// <list type="bullet">
/// <item>A Docker network connecting all containers (inherited from <see cref="MessagingTestFixtures"/>)</item>
/// <item>A RabbitMQ container for messaging (inherited from <see cref="MessagingTestFixtures"/>)</item>
/// <item>A Server application container built from source</item>
/// <item>A Client application container built from source</item>
/// </list>
/// <para>
/// Used by <see cref="EndToEndContainersTests"/> to test the complete system with all components
/// running in containers, simulating a production-like environment.
/// </para>
/// </summary>
public class EndToEndTestFixtures : MessagingTestFixtures
{
    /// <summary>
    /// The Server application container fixture.
    /// <para>
    /// This container is built dynamically from the Server project's source code and Dockerfile.
    /// It provides methods for retrieving logs and verifying expected behavior.
    /// </para>
    /// </summary>
    public ServerFixture ServerFixture { get; } = new();

    /// <summary>
    /// The Client application container fixture.
    /// <para>
    /// This container is built dynamically from the Client project's source code and Dockerfile.
    /// It exposes the hostname and port needed to make HTTP requests to the containerized Client.
    /// </para>
    /// </summary>
    public ClientFixture ClientFixture { get; } = new();

    /// <inheritdoc/>
    /// <remarks>
    /// <para>
    /// The initialization sequence for EndToEndTestFixtures:
    /// </para>
    /// <list type="number">
    /// <item>The Docker network is created first</item>
    /// <item>RabbitMQ container is started and connected to the network</item>
    /// <item>Server and Client containers are built from source and started in parallel</item>
    /// <item>All containers are connected to the same network for communication</item>
    /// </list>
    /// </remarks>
    protected override async Task InitializeAfterNetworkAsync()
    {
        await Task.WhenAll(
            base.InitializeAfterNetworkAsync(),
            ServerFixture.InitializeAsync(NetworkFixture.Network),
            ClientFixture.InitializeAsync(NetworkFixture.Network));
    }

    /// <inheritdoc/>
    /// <remarks>
    /// <para>
    /// The disposal sequence for EndToEndTestFixtures:
    /// </para>
    /// <list type="number">
    /// <item>Client and Server containers are stopped and removed</item>
    /// <item>RabbitMQ container is stopped and removed</item>
    /// <item>Docker network is removed last</item>
    /// </list>
    /// </remarks>
    protected override async Task DisposeBeforeNetworkAsync()
    {
        await Task.WhenAll(
            base.DisposeBeforeNetworkAsync(),
            ClientFixture.DisposeAsync().AsTask(),
            ServerFixture.DisposeAsync().AsTask());
    }
}
