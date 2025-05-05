using IntegrationTests.Configuration.Factories;
using IntegrationTests.Configuration.Fixtures;

namespace IntegrationTests.Tests.Testcontainers.Fixtures;

/// <summary>
/// Test fixtures for Server integration tests using a hybrid approach with Testcontainers.
/// <para>
/// This fixture provides infrastructure for a testing setup where:
/// </para>
/// <list type="bullet">
/// <item>The Server application runs in-process via WebApplicationFactory</item>
/// <item>The Client application runs in a Docker container</item>
/// <item>RabbitMQ runs in a Docker container (inherited from <see cref="MessagingTestFixtures"/>)</item>
/// <item>All components connect via a Docker network (inherited from <see cref="MessagingTestFixtures"/>)</item>
/// </list>
/// <para>
/// This hybrid approach enables deeper testing and mocking of the Server component
/// while still validating its integration with containerized dependencies.
/// </para>
/// </summary>
public class ServerTestFixtures : MessagingTestFixtures
{
    /// <summary>
    /// The in-process Server application factory.
    /// <para>
    /// This factory creates an in-memory test server that hosts the Server application,
    /// allowing tests to mock and verify internal Server behavior while it processes
    /// messages and interacts with other components.
    /// </para>
    /// </summary>
    public ServerIntegrationTestWebAppFactory Factory { get; } = new();

    /// <summary>
    /// The Client application container fixture.
    /// <para>
    /// This container runs the Client application and is built dynamically from the project's source code.
    /// It exposes the hostname and port needed to make HTTP requests to the containerized Client.
    /// </para>
    /// </summary>
    public ClientFixture ClientFixture { get; } = new();

    /// <inheritdoc/>
    /// <remarks>
    /// <para>
    /// The initialization sequence for ServerTestFixtures:
    /// </para>
    /// <list type="number">
    /// <item>The Docker network is created first (handled by base class)</item>
    /// <item>RabbitMQ container is initialized (handled by base class)</item>
    /// <item>Client container is built from source and started</item>
    /// <item>Server WebApplicationFactory is configured to use the containerized RabbitMQ instance</item>
    /// </list>
    /// </remarks>
    protected override async Task InitializeAfterNetworkAsync()
    {
        await Task.WhenAll(
            base.InitializeAfterNetworkAsync(),
            ClientFixture.InitializeAsync(NetworkFixture.Network));

        Factory.ConfigureRabbitMq(RabbitMqFixture);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// <para>
    /// The disposal sequence for ServerTestFixtures:
    /// </para>
    /// <list type="number">
    /// <item>RabbitMQ container is stopped (handled by base class)</item>
    /// <item>Client container is stopped and removed</item>
    /// <item>Server WebApplicationFactory is disposed</item>
    /// <item>Docker network is removed last (handled by base class)</item>
    /// </list>
    /// </remarks>
    protected override async Task DisposeBeforeNetworkAsync()
    {
        await Task.WhenAll(
            base.DisposeBeforeNetworkAsync(),
            ClientFixture.DisposeAsync().AsTask(),
            Factory.DisposeAsync().AsTask());
    }
}
