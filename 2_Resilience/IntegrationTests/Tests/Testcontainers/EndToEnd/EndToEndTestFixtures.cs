using IntegrationTests.Configuration.Fixtures;
using Xunit;

namespace IntegrationTests.Tests.Testcontainers.EndToEnd;

public class EndToEndTestFixtures : IAsyncLifetime
{
    public NetworkFixture NetworkFixture { get; } = new();
    public RabbitMqFixture RabbitMqFixture { get; } = new();
    public ServerFixture ServerFixture { get; } = new();
    public ClientFixture ClientFixture { get; } = new();

    public async Task InitializeAsync()
    {
        await Task.WhenAll(
            RabbitMqFixture.InitializeAsync(NetworkFixture.Network),
            ServerFixture.InitializeAsync(NetworkFixture.Network),
            ClientFixture.InitializeAsync(NetworkFixture.Network));
    }

    public async Task DisposeAsync()
    {
        await Task.WhenAll(
            ClientFixture.DisposeAsync().AsTask(),
            ServerFixture.DisposeAsync().AsTask(),
            RabbitMqFixture.DisposeAsync().AsTask());
        await NetworkFixture.DisposeAsync();
    }
}