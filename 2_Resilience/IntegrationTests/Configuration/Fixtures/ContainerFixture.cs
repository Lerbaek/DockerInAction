using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using IntegrationTests.Configuration.Factories;
using Xunit;

namespace IntegrationTests.Configuration.Fixtures;

public abstract class ContainerFixture<T> : IAsyncLifetime
{
    public static INetwork? Network { get; } = NetworkFixture<T>.Instance.Network;

    static ContainerFixture()
    {
        // Set the resource reaper image to on resolvable by a Danske Bank machine.
        TestcontainersSettings.ResourceReaperImage = new DockerImage("remote-docker-hub.artifactory.danskenet.net/testcontainers/ryuk:0.5.1");
    }

    protected abstract IContainer Container { get; }

    /// <summary>
    /// Create a builder for a container configured with RabbitMQ within the same <see cref="INetwork"/>.
    /// </summary>
    /// <param name="image">The image on which the container is based.</param>
    /// <param name="network">The network through which the RabbitMQ container can be reached.</param>
    /// <returns></returns>
    protected ContainerBuilder CreateRabbitMqConfiguredContainer(IImage image, INetwork network)
    {
        return new ContainerBuilder()
            .WithName($"testcontainers-{image.Name}")
            .WithImage(image)
            .WithNetwork(network)
            .WithEnvironment("RabbitMq:Host", nameof(RabbitMQ));
    }

    public virtual async Task InitializeAsync()
    {
        await Container.StartAsync();
    }

    public virtual async Task DisposeAsync() => await Container.DisposeAsync();
}