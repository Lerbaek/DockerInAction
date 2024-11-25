using DotNet.Testcontainers.Containers;

namespace IntegrationTests.Configuration.Fixtures;

public sealed class ServerFixture() : ImageFixture(nameof(Tests.Server))
{
    private IContainer? _container;

    protected override IContainer Container
    {
        get
        {
            _container ??= CreateRabbitMqConfiguredContainer(Image)
                .WithHostname(nameof(Tests.Server))
                .Build();
            return _container;
        }
    }

    public async Task<(string Stdout, string Stderr)> GetLogsAsync(DateTime since) => await Container.GetLogsAsync(since, timestampsEnabled: false);
}