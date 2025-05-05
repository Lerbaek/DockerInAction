using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using FluentAssertions;
using Xunit.Abstractions;

namespace IntegrationTests.Configuration.Fixtures;

/// <summary>
/// Fixture for creating a containerized Server application instance.
/// <para>
/// This fixture builds and runs the Server application in a Docker container
/// using the project's source code and Dockerfile.
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// The container is configured to wait until the Server's message bus is started,
/// ensuring it's ready for use when tests begin.
/// </para>
/// <para>
/// This fixture provides methods for retrieving and validating Server logs, which is
/// useful for verifying the Server's behavior in integration tests.
/// </para>
/// </remarks>
public sealed class ServerFixture() : ImageFixture(nameof(Server))
{
    /// <inheritdoc/>
    protected override IContainer BuildContainer(INetwork network) =>
        CreateRabbitMqConfiguredContainerBuilder(Image, network)
            .WithHostname(nameof(Server))
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(".*Bus started.*"))
            .Build();

    /// <summary>
    /// Gets the logs from the Server container since a specific time.
    /// </summary>
    /// <param name="since">The starting time to retrieve logs from.</param>
    /// <returns>A tuple containing the standard output and standard error logs.</returns>
    private async Task<(string Stdout, string Stderr)> GetLogsAsync(DateTime since) =>
        await Container.GetLogsAsync(since, timestampsEnabled: false);
    
    /// <summary>
    /// Asserts that the Server logs contain expected success or failure messages.
    /// <para>
    /// This method waits up to 10 seconds for the expected log messages to appear,
    /// then verifies that they exist in the log output.
    /// </para>
    /// </summary>
    /// <param name="expectSuccess">
    /// The expected outcome: true for success messages only, false for failure
    /// messages only, or null for either success or failure messages.
    /// </param>
    /// <param name="startTime">The time when the test started, to filter older logs.</param>
    /// <param name="initialLogLength">The initial length of logs to skip.</param>
    /// <param name="output">The test output helper for writing logs to test results.</param>
    public void AssertServerLogSuccess(
        bool? expectSuccess,
        DateTime startTime,
        int initialLogLength,
        ITestOutputHelper output)
    {
        var log = string.Empty;
        var validLogMessages = GetValidLogMessages(expectSuccess);

        SpinWait.SpinUntil(() =>
        {
            log = GetLogsAsync(since: startTime)
                .Result.Stdout[initialLogLength..];

            return Array.Exists(
                validLogMessages,
                validLogMessage => log.Contains(validLogMessage));
        }, TimeSpan.FromSeconds(10));

        output.WriteLine(string.Empty);
        output.WriteLine(log);
        log.Should().ContainAny(validLogMessages);
    }

    /// <summary>
    /// Gets the expected log messages based on the expected success outcome.
    /// </summary>
    /// <param name="expectSuccess">
    /// The expected outcome: true for success messages only, false for failure
    /// messages only, or null for either success or failure messages.
    /// </param>
    /// <returns>An array of valid log message strings to look for.</returns>
    public static string[] GetValidLogMessages(bool? expectSuccess)
    {
        const string expectedSuccessMessage = "Payment succeeded";
        const string expectedFailureMessage = "Payment failed";

        return expectSuccess switch
        {
            true  => [expectedSuccessMessage],
            false => [expectedFailureMessage],
            null  => [expectedSuccessMessage, expectedFailureMessage]
        };
    }

    /// <summary>
    /// Gets the length of logs at a specific point in time.
    /// <para>
    /// This method is useful for establishing a baseline log length before
    /// performing actions that generate new logs.
    /// </para>
    /// </summary>
    /// <param name="startTime">The time to retrieve logs from.</param>
    /// <returns>The length of the log string at the specified time.</returns>
    public async Task<int> GetLogLength(DateTime startTime)
    {
        var (stdout, _) = await GetLogsAsync(since: startTime);
        var initialLogLength = stdout.Length;
        return initialLogLength;
    }
}