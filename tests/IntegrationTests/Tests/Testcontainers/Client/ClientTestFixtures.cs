using IntegrationTests.Configuration.Factories;
using IntegrationTests.Configuration.Fixtures;
using Xunit;

namespace IntegrationTests.Tests.Testcontainers.Client;

public class ClientTestFixtures : IAsyncLifetime
{
    public ClientIntegrationTestWebAppFactory Factory { get; } = new();
    public NetworkFixture NetworkFixture { get; } = new();
    public RabbitMqFixture RabbitMqFixture { get; } = new();
    public ServerFixture ServerFixture { get; } = new();

    public async Task InitializeAsync()
    {
        await NetworkFixture.InitializeAsync();
        await Task.WhenAll(
            RabbitMqFixture.InitializeAsync(NetworkFixture.Network),
            ServerFixture.InitializeAsync(NetworkFixture.Network));
    }

    public async Task DisposeAsync()
    {
        await Task.WhenAll(
            ServerFixture.DisposeAsync().AsTask(),
            RabbitMqFixture.DisposeAsync().AsTask(),
            Factory.DisposeAsync().AsTask());
        await NetworkFixture.DisposeAsync();
    }
}