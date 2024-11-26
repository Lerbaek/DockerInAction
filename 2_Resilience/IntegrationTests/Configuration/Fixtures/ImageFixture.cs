using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using IntegrationTests.Configuration.Factories;
using Testcontainers.RabbitMq;

namespace IntegrationTests.Configuration.Fixtures;

public abstract class ImageFixture(string projectName) : ContainerFixture
{
    protected readonly IFutureDockerImage Image = new ImageFromDockerfileBuilder()
        .WithName($"{projectName.ToLowerInvariant()}-{Guid.NewGuid()}")
        .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory().DirectoryPath)
        .WithDockerfile($"{projectName}/Dockerfile")
        .Build();

    public override async Task InitializeAsync(INetwork network)
    {
        await Image.CreateAsync();
        await base.InitializeAsync(network);
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await Image.DisposeAsync();
    }
}