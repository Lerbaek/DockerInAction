using DotNet.Testcontainers.Containers;

namespace IntegrationTests.Configuration.Fixtures;

public sealed class ClientFixture() : ImageFixture(nameof(Tests.Client))
{
    private IContainer? _container;

    public string Hostname => Container.Hostname;
    public object Port => Container.GetMappedPublicPort(8080);

    protected override IContainer Container
    {
        get
        {
            _container ??= CreateRabbitMqConfiguredContainer(Image)
                .WithPortBinding(8080, assignRandomHostPort: true)
                .WithHostname(nameof(Tests.Client))
                .Build();
            return _container;
        }
    }
}