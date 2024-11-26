using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using IntegrationTests.Configuration.Factories;

namespace IntegrationTests.Configuration.Fixtures;

public sealed class ClientFixture() : ImageFixture(nameof(Client))
{
    protected override IContainer BuildContainer(INetwork network) =>
        CreateRabbitMqConfiguredContainerBuilder(Image, network)
            .WithPortBinding(8080, assignRandomHostPort: true)
            .WithHostname(nameof(Client))
            .Build();

    public string? Hostname => Container.Hostname;

    public ushort? Port => Container.GetMappedPublicPort(8080);
}