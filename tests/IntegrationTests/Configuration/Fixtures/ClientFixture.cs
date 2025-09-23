using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Configuration.Fixtures;

/// <summary>
/// Fixture for creating a containerized Client application instance.
/// <para>
/// This fixture builds and runs the Client application in a Docker container
/// using the project's source code and Dockerfile.
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// The container exposes the Client's HTTP endpoint on port 8080 and maps it to
/// a random host port for test access.
/// </para>
/// <para>
/// This fixture is primarily used in hybrid testing approaches where the Server
/// runs in-process and the Client runs in a container, or in end-to-end tests
/// where both Client and Server run in containers.
/// </para>
/// </remarks>
public sealed class ClientFixture() : ImageFixture(nameof(Client))
{
    private ServiceProvider? _serviceProvider;

    /// <inheritdoc/>
    protected override IContainer BuildContainer(INetwork network) =>
        CreateRabbitMqConfiguredContainerBuilder(Image, network)
            .WithPortBinding(8080, assignRandomHostPort: true)
            .WithHostname(nameof(Client))
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilHttpRequestIsSucceeded(strategy =>
                        strategy
                            .UsingTls(false)
                            .ForPath("/health")
                            .ForPort(8080)))
            .Build();

    /// <summary>
    /// Gets an <see cref="HttpClient"/> configured to communicate with the Client application
    /// </summary>
    public HttpClient HttpClient =>
        _serviceProvider?
            .GetRequiredService<IHttpClientFactory>()
            .CreateClient(nameof(ClientFixture))
        ?? throw NotInitializedException;

    /// <remarks>
    /// Extends the base initialization to register and configure an
    /// <see cref="HttpClient"/> targeting the Client application.
    /// </remarks>
    /// <inheritdoc/>
    public override async Task InitializeAsync(INetwork network)
    {
        await base.InitializeAsync(network);

        var services = new ServiceCollection();

        services.AddHttpClient<ClientFixture>(c =>
            c.BaseAddress = new Uri(
                $"http://{Container.Hostname}:{(ushort?)Container.GetMappedPublicPort(8080)}"));

        _serviceProvider = services.BuildServiceProvider();
    }

    /// <remarks>
    /// Extends the base disposal to dispose of the <see cref="ServiceProvider"/> beforing cleaning up the Docker image and container.
    /// </remarks>
    /// <inheritdoc/>
    public override async ValueTask DisposeAsync()
    {
        if(_serviceProvider is {})
        {
            await _serviceProvider.DisposeAsync();
            _serviceProvider = null;
        }

        await base.DisposeAsync();
    }
}