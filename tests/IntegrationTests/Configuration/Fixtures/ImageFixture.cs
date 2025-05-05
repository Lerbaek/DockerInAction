using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;

namespace IntegrationTests.Configuration.Fixtures;

/// <summary>
/// Base abstract class for container fixtures that build images from project source code.
/// <para>
/// This class extends <see cref="ContainerFixture"/> to provide functionality for
/// building Docker images dynamically from the project's source code and Dockerfiles.
/// </para>
/// </summary>
/// <param name="projectName">The name of the project to build an image for.</param>
public abstract class ImageFixture(string projectName) : ContainerFixture
{
    /// <summary>
    /// The Docker image to be built from the project's Dockerfile.
    /// <para>
    /// This image is built dynamically at test runtime using the project's source code
    /// and Dockerfile, ensuring tests run against the latest version of the code.
    /// </para>
    /// </summary>
    protected readonly IFutureDockerImage Image = new ImageFromDockerfileBuilder()
        .WithName($"{projectName.ToLowerInvariant()}-{Guid.NewGuid()}")
        .WithDockerfileDirectory($"{CommonDirectoryPath.GetSolutionDirectory().DirectoryPath}\\src")
        .WithDockerfile($"{projectName}/Dockerfile")
        .Build();

    /// <inheritdoc/>
    /// <remarks>
    /// Extends the base initialization to first build the Docker image before creating
    /// and starting the container.
    /// </remarks>
    public override async Task InitializeAsync(INetwork network)
    {
        await Image.CreateAsync();
        await base.InitializeAsync(network);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Extends the base disposal to clean up the Docker image after the container is disposed.
    /// </remarks>
    public override async ValueTask DisposeAsync()
    {
        await Image.DisposeAsync();
        await base.DisposeAsync();
    }
}