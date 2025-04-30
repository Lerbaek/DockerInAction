using DotNet.Testcontainers;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit.Sdk;

namespace IntegrationTests.Configuration.Fixtures;

public abstract class ImageFixture(string projectName) : ContainerFixture
{
    protected readonly IFutureDockerImage Image = new ImageFromDockerfileBuilder()
        .WithName($"{projectName.ToLowerInvariant()}-{Guid.NewGuid()}")
        .WithDockerfileDirectory($"{CommonDirectoryPath.GetSolutionDirectory().DirectoryPath}\\src")
        .WithDockerfile($"{projectName}/Dockerfile")
        .Build();

    public override async Task InitializeAsync(INetwork network)
    {
        await Image.CreateAsync();
        await base.InitializeAsync(network);
    }

    public override async ValueTask DisposeAsync()
    {
        await Image.DisposeAsync();
        await base.DisposeAsync();
    }
}