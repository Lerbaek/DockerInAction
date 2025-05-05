using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;

namespace IntegrationTests.Configuration.Fixtures;

/// <summary>
/// Base abstract class for container fixtures used in integration tests.
/// <para>
/// This class provides the foundation for creating and managing Docker containers
/// during test execution, handling container lifecycle and configuration.
/// </para>
/// </summary>
/// <remarks>
/// ContainerFixture implements IAsyncDisposable to ensure proper cleanup of Docker resources.
/// Derived classes must implement <see cref="BuildContainer"/> to specify container configuration.
/// </remarks>
public abstract class ContainerFixture : IAsyncDisposable
{
    private IContainer? _container;

    /// <summary>
    /// Exception thrown when attempting to access a container before it has been initialized.
    /// </summary>
    public class ContainerNotInitializedException(string message) : Exception(message);
    
    /// <summary>
    /// Static constructor to configure Testcontainers settings.
    /// </summary>
    static ContainerFixture()
    {
        // When run from a company machine, set the resource reaper image to one from the internal image repository.
        // When not, this line is not needed.
        TestcontainersSettings.ResourceReaperImage = new DockerImage("remote-docker-hub.artifactory.danskenet.net/testcontainers/ryuk:0.11.0");
    }

    /// <summary>
    /// Creates the container with the specified configuration.
    /// </summary>
    /// <param name="network">The Docker network to connect the container to.</param>
    /// <returns>A configured container instance ready to be started.</returns>
    protected abstract IContainer BuildContainer(INetwork network);

    /// <summary>
    /// Gets the container instance if it has been initialized.
    /// </summary>
    /// <exception cref="ContainerNotInitializedException">Thrown if the container has not been initialized.</exception>
    protected IContainer Container
    {
        get => _container ?? throw new ContainerNotInitializedException($"{GetType().Name} has not yet been initialized.");
        set => _container = value;
    }

    /// <summary>
    /// Creates a container builder configured with RabbitMQ connection settings.
    /// <para>
    /// This helper method simplifies creating containers that need to communicate with RabbitMQ
    /// by preconfiguring the environment variables and network settings.
    /// </para>
    /// </summary>
    /// <param name="image">The Docker image to use for the container.</param>
    /// <param name="network">The Docker network to connect the container to.</param>
    /// <returns>A container builder with basic RabbitMQ configuration applied.</returns>
    protected ContainerBuilder CreateRabbitMqConfiguredContainerBuilder(IFutureDockerImage image, INetwork network)
    {
        return new ContainerBuilder()
            .WithName($"testcontainers-{image.Repository}")
            .WithImage(image)
            .WithNetwork(network)
            .WithEnvironment("RabbitMq:Host", nameof(RabbitMQ));
    }
    
    /// <summary>
    /// Initializes the container by building and starting it.
    /// </summary>
    /// <param name="network">The Docker network to connect the container to.</param>
    /// <returns>A task representing the asynchronous initialization operation.</returns>
    public virtual async Task InitializeAsync(INetwork network)
    {
        _container = BuildContainer(network);
        await _container.StartAsync();
    }

    /// <summary>
    /// Disposes the container, stopping and removing it from Docker.
    /// </summary>
    /// <returns>A task representing the asynchronous disposal operation.</returns>
    public virtual async ValueTask DisposeAsync()
    {
        if (_container is { })
        {
            await _container.DisposeAsync();
            _container = null;
        }
    }
}