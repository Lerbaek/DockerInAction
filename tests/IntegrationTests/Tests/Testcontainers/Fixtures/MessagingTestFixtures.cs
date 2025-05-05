using IntegrationTests.Configuration.Fixtures;
using Xunit;

namespace IntegrationTests.Tests.Testcontainers.Fixtures;

/// <summary>
/// Base fixture class for integration tests requiring messaging infrastructure.
/// <para>
/// This class provides the foundational infrastructure for any test that requires 
/// a Docker network and a RabbitMQ message broker. It manages:
/// </para>
/// <list type="bullet">
/// <item>A Docker network that enables communication between containerized services</item>
/// <item>A RabbitMQ container configured with the management plugin</item>
/// </list>
/// <para>
/// Derived classes can extend this infrastructure by adding additional containerized services
/// within the same network, enabling tests that verify communication between components.
/// </para>
/// </summary>
/// <remarks>
/// This class handles proper initialization and cleanup order, ensuring that the network
/// is created first and cleaned up last, while containers are initialized and disposed in the
/// correct sequence to maintain consistency.
/// </remarks>
public class MessagingTestFixtures : IAsyncLifetime
{
    /// <summary>
    /// The Docker network fixture that connects all containers.
    /// <para>
    /// This network is created with a unique name to avoid conflicts with other test runs.
    /// All containers created by this fixture or derived fixtures are connected to this network,
    /// allowing them to communicate with each other using their container names.
    /// </para>
    /// </summary>
    public NetworkFixture NetworkFixture { get; } = new();

    /// <summary>
    /// The RabbitMQ container fixture for messaging between services.
    /// <para>
    /// This container runs RabbitMQ with the management plugin enabled and provides
    /// the hostname and port needed to connect to the broker.
    /// </para>
    /// </summary>
    public RabbitMqFixture RabbitMqFixture { get; } = new();

    /// <summary>
    /// Initializes the test infrastructure in the correct order.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The initialization sequence is:
    /// </para>
    /// <list type="number">
    /// <item>Create the Docker network</item>
    /// <item>Initialize dependent containers (RabbitMQ and any containers added by derived classes)</item>
    /// </list>
    /// </remarks>
    /// <returns>A task representing the asynchronous initialization operation.</returns>
    public async Task InitializeAsync()
    {
        await NetworkFixture.InitializeAsync();
        await InitializeAfterNetworkAsync();
    }

    /// <summary>
    /// Initializes containers after the network has been created.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This virtual method allows derived classes to add additional initialization logic
    /// while ensuring that the network is created first.
    /// </para>
    /// <para>
    /// The base implementation initializes the RabbitMQ container and connects it to the network.
    /// </para>
    /// </remarks>
    /// <returns>A task representing the asynchronous container initialization.</returns>
    protected virtual async Task InitializeAfterNetworkAsync()
    {
        await RabbitMqFixture.InitializeAsync(NetworkFixture.Network);
    }

    /// <summary>
    /// Disposes the test infrastructure in the correct order.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The disposal sequence is:
    /// </para>
    /// <list type="number">
    /// <item>Dispose of all containers (RabbitMQ and any added by derived classes)</item>
    /// <item>Finally, dispose of the Docker network</item>
    /// </list>
    /// </remarks>
    /// <returns>A task representing the asynchronous disposal operation.</returns>
    public async Task DisposeAsync()
    {
        await DisposeBeforeNetworkAsync();
        await NetworkFixture.DisposeAsync();
    }

    /// <summary>
    /// Disposes containers before the network is removed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This virtual method allows derived classes to add additional disposal logic
    /// while ensuring that all containers are disposed before the network.
    /// </para>
    /// <para>
    /// The base implementation disposes of the RabbitMQ container.
    /// </para>
    /// </remarks>
    /// <returns>A task representing the asynchronous container disposal.</returns>
    protected virtual async Task DisposeBeforeNetworkAsync()
    {
        await RabbitMqFixture.DisposeAsync();
    }
}
