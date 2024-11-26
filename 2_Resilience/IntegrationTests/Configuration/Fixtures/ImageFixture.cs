using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using IntegrationTests.Configuration.Factories;
using Testcontainers.RabbitMq;

namespace IntegrationTests.Configuration.Fixtures;

public abstract class ImageFixture<T>(string projectName) : ContainerFixture<T>
{
    protected readonly IFutureDockerImage Image = new ImageFromDockerfileBuilder()
        .WithName($"{projectName.ToLowerInvariant()}-{Guid.NewGuid()}")
        .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory().DirectoryPath)
        .WithDockerfile($"{projectName}/Dockerfile")
        .Build();

    public override async Task InitializeAsync()
    {
        await Image.CreateAsync();
        await base.InitializeAsync();
    }

    public override async Task DisposeAsync()
    {
        await Image.DisposeAsync();
        await base.DisposeAsync();
    }
}