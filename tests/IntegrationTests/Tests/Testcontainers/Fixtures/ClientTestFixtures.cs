using IntegrationTests.Configuration.Factories;
using IntegrationTests.Configuration.Fixtures;

namespace IntegrationTests.Tests.Testcontainers.Fixtures;

/// <summary>
/// Test fixtures for Client integration tests using a hybrid approach with Testcontainers.
/// <para>
/// This fixture provides infrastructure for a testing setup where:
/// </para>
/// <list type="bullet">
/// <item>The Client application runs in-process via WebApplicationFactory</item>
/// <item>The Server application runs in a Docker container</item>
/// <item>RabbitMQ runs in a Docker container (inherited from <see cref="MessagingTestFixtures"/>)</item>
/// <item>All components connect via a Docker network (inherited from <see cref="MessagingTestFixtures"/>)</item>
/// </list>
/// <para>
/// This hybrid approach enables faster tests with more debugging capabilities for the Client component
/// while still testing its integration with real containerized dependencies.
/// </para>
/// </summary>
public class ClientTestFixtures : MessagingTestFixtures
{
    /// <summary>
    /// The in-process Client application factory.
    /// <para>
    /// This factory creates an in-memory test server that hosts the Client application,
    /// allowing tests to make HTTP requests to the Client and verify its behavior.
    /// </para>
    /// </summary>
    public ClientIntegrationTestWebAppFactory Factory { get; } = new();

    /// <summary>
    /// The Server application container fixture.
    /// <para>
    /// This container runs the Server application and is built dynamically from the project's source code.
    /// It provides methods for retrieving logs and verifying expected behavior.
    /// </para>
    /// </summary>
    public ServerFixture ServerFixture { get; } = new();

    /// <inheritdoc/>
    /// <remarks>
    /// <para>
    /// The initialization sequence for ClientTestFixtures:
    /// </para>
    /// <list type="number">
    /// <item>The Docker network is created first (handled by base class)</item>
    /// <item>RabbitMQ container is initialized (handled by base class)</item>
    /// <item>Server container is built from source and started</item>
    /// <item>Client WebApplicationFactory is configured to use the containerized RabbitMQ instance</item>
    /// </list>
    /// </remarks>
    protected override async Task InitializeAfterNetworkAsync()
    {
        await Task.WhenAll(
            base.InitializeAfterNetworkAsync(),
            ServerFixture.InitializeAsync(NetworkFixture.Network));

        Factory.ConfigureRabbitMq(RabbitMqFixture);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// <para>
    /// The disposal sequence for ClientTestFixtures:
    /// </para>
    /// <list type="number">
    /// <item>RabbitMQ container is stopped (handled by base class)</item>
    /// <item>Server container is stopped and removed</item>
    /// <item>Client WebApplicationFactory is disposed</item>
    /// <item>Docker network is removed last (handled by base class)</item>
    /// </list>
    /// </remarks>
    protected override async Task DisposeBeforeNetworkAsync()
    {
        await Task.WhenAll(
            base.DisposeBeforeNetworkAsync(),
            ServerFixture.DisposeAsync().AsTask(),
            Factory.DisposeAsync().AsTask());
    }
}
