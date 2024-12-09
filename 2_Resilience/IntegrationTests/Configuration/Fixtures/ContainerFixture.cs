using System.Configuration;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;

namespace IntegrationTests.Configuration.Fixtures;

public abstract class ContainerFixture : IAsyncDisposable
{
    private IContainer? _container;
    
    static ContainerFixture()
    {
        // When run from a company machine, set the resource reaper image to one from the internal image repository.
        // When not, this line is not needed.
        TestcontainersSettings.ResourceReaperImage = new DockerImage("testcontainers/ryuk:0.5.1");
    }

    protected abstract IContainer BuildContainer(INetwork network);

    protected IContainer Container
    {
        get => _container ?? throw new ConfigurationErrorsException($"{GetType().Name} has not yet been initialized.");
        set => _container = value;
    }

    /// <summary>
    /// Create a builder for a container configured with RabbitMQ within the same <see cref="INetwork"/>.
    /// </summary>
    /// <param name="image">The image on which the container is based.</param>
    /// <returns></returns>
    protected ContainerBuilder CreateRabbitMqConfiguredContainerBuilder(IFutureDockerImage image, INetwork network)
    {
        return new ContainerBuilder()
            .WithName($"testcontainers-{image.Name}")
            .WithImage(image)
            .WithNetwork(network)
            .WithEnvironment("RabbitMq:Host", nameof(RabbitMQ));
    }
    
    /// <param name="network">The network through which the RabbitMQ container can be reached.</param>
    public virtual async Task InitializeAsync(INetwork network)
    {
        _container = BuildContainer(network);
        await _container.StartAsync();
    }

    public virtual async ValueTask DisposeAsync()
    {
        if (_container is not null)
        {
            await _container.DisposeAsync();
            _container = null;
        }
    }
}