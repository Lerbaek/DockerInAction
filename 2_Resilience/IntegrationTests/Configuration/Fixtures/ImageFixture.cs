using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Images;

namespace IntegrationTests.Configuration.Fixtures;

public abstract class ImageFixture : ContainerFixture
{
    protected readonly IFutureDockerImage Image;

    protected ImageFixture(string projectName)
    {
        Image = new ImageFromDockerfileBuilder()
            .WithName($"{projectName.ToLowerInvariant()}-{Guid.NewGuid()}")
            .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory().DirectoryPath)
            .WithDockerfile($"{projectName}/Dockerfile")
            .Build();
    }

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