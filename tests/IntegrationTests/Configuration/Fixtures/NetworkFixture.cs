using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using Xunit;

namespace IntegrationTests.Configuration.Fixtures;

public class NetworkFixture : IAsyncLifetime
{
    public INetwork Network { get; } = new NetworkBuilder()
        .WithName($"testcontainers-microservices-{Guid.NewGuid()}")
        //.WithCleanUp(true)
        .Build();

    public async Task InitializeAsync()
    {
        await Network.CreateAsync();
    }

    public async Task DisposeAsync()
    {
        await Network.DisposeAsync();
    }
}