using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using FluentAssertions;
using IntegrationTests.Configuration.Factories;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Xunit.Abstractions;

namespace IntegrationTests.Configuration.Fixtures;

public sealed class ServerFixture() : ImageFixture(nameof(Server))
{
    const string ExpectedSuccessMessage = "Payment succeeded";
    const string ExpectedFailureMessage = "Payment failed";

    private IContainer? _container;

    protected override IContainer BuildContainer(INetwork network) =>
        CreateRabbitMqConfiguredContainerBuilder(Image, network)
            .WithHostname(nameof(Server))
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(".*Bus started.*"))
            .Build();

    private async Task<(string Stdout, string Stderr)> GetLogsAsync(DateTime since) => await Container.GetLogsAsync(since, timestampsEnabled: false);
    
    public void AssertServerLogSuccess(bool expectSuccess, DateTime startTime, int initialLogLength, ITestOutputHelper output)
    {
        var log = string.Empty;
        var expectedOutput = expectSuccess ? ExpectedSuccessMessage : ExpectedFailureMessage;

        SpinWait.SpinUntil(() =>
        {
            log = GetLogsAsync(since: startTime)
                .Result.Stdout[initialLogLength ..];
            return log.Contains(expectedOutput);
        }, TimeSpan.FromSeconds(10));

        output.WriteLine(log);
        log.Should().Contain(expectedOutput);
    }

    public async Task<int> GetLogLength(DateTime startTime)
    {
        var initialLog = await GetLogsAsync(since: startTime);
        var initialLogLength = initialLog.Stdout.Length;
        return initialLogLength;
    }
}