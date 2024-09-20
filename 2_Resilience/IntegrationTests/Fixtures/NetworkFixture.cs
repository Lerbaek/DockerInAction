using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using Xunit;

namespace IntegrationTests.Fixtures;

public sealed class NetworkFixture : IAsyncLifetime
{
    private readonly INetwork _network;

    public NetworkFixture()
    {
        Name = $"testcontainers-network-{Guid.NewGuid()}";
        _network = new NetworkBuilder()
            .WithName(Name)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _network.CreateAsync();
    }

    public async Task DisposeAsync()
    {
        await _network.DisposeAsync();
    }

    public static string? Name { get; private set; }
}