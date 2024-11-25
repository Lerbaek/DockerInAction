using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using Xunit;

namespace IntegrationTests.Configuration.Fixtures;

public abstract class ContainerFixture : IAsyncLifetime
{
    protected static readonly INetwork Network = new NetworkBuilder()
        .WithName($"testcontainers-microservices-{Guid.NewGuid()}")
        .Build();

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
    /// <returns></returns>
    protected ContainerBuilder CreateRabbitMqConfiguredContainer(IImage image)
    {
        return new ContainerBuilder()
            .WithName($"testcontainers-{image.Name}")
            .WithImage(image)
            .WithNetwork(Network)
            .WithEnvironment("RabbitMq:Host", nameof(RabbitMQ));
    }

    public virtual async Task InitializeAsync() => await Container.StartAsync();

    public virtual async Task DisposeAsync() => await Container.DisposeAsync();
}