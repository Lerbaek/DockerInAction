using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using IntegrationTests.Configuration.Factories;

namespace IntegrationTests.Configuration.Fixtures;

public sealed class ClientFixture<T>() : ImageFixture<T>(nameof(Client))
{
    private IContainer? _container;

    protected override IContainer Container =>
        _container ??= CreateRabbitMqConfiguredContainer(Image, Network)
            .WithPortBinding(8080, assignRandomHostPort: true)
            .WithHostname(nameof(Client))
            .Build();

    public string? Hostname => Container.Hostname;

    public ushort? Port => Container.GetMappedPublicPort(8080);
}