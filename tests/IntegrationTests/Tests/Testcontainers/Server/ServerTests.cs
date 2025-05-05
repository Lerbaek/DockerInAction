using IntegrationTests.Configuration.Collections;
using IntegrationTests.Configuration.Fixtures;
using IntegrationTests.Tests.Testcontainers.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.Community.Logging;
using Shared;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests.Tests.Testcontainers.Server;

/// <summary>
/// Integration tests for the Server application using Testcontainers.
/// <para>
/// These tests validate the Server's behavior when interacting with the Client and RabbitMQ.
/// The Server is tested in a hybrid setup where:
/// </para>
/// <list type="bullet">
/// <item>The Client application runs in a container</item>
/// <item>The Server application runs in-process via WebApplicationFactory</item>
/// <item>RabbitMQ runs in a Docker container</item>
/// </list>
/// <para>
/// This approach allows for testing the Server's integration with containerized dependencies
/// while maintaining the ability to mock and verify internal Server behavior.
/// </para>
/// </summary>
[Collection(nameof(ServerTestcontainersCollection))]
public class ServerTests
{
    private readonly ITestOutputHelper _output;
    private readonly ServerTestFixtures _fixtures;

    /// <inheritdoc cref="ServerTests"/>
    /// <param name="output">Output helper for writing to test logs.</param>
    /// <param name="fixtures">Test fixtures providing access to test dependencies.</param>
    public ServerTests(
        ITestOutputHelper output,
        ServerTestFixtures fixtures)
    {
        _output = output;
        _fixtures = fixtures;

        // Configure the in-process Server app to use the containerized RabbitMQ instance
        fixtures.Factory.RabbitMqOptions.Port = fixtures.RabbitMqFixture.Port;
        fixtures.Factory.RabbitMqOptions.Host = fixtures.RabbitMqFixture.Hostname;
    }

    /// <summary>
    /// Tests that the Server correctly processes HTTP GET requests to the PaymentGenerator endpoint.
    /// <para>
    /// The test verifies that:
    /// </para>
    /// <list type="bullet">
    /// <item>The Client application successfully sends HTTP requests to the Server</item>
    /// <item>The Server processes the requests and interacts with RabbitMQ</item>
    /// <item>The Server logs the expected success or failure messages based on the specified stability mode</item>
    /// <item>The Server's internal logging behavior can be mocked and verified</item>
    /// </list>
    /// </summary>
    /// <param name="serverStability">Controls the Server's behavior: Functional (success), Flaky (inconsistent), or Failing (error).</param>
    /// <param name="expectSuccess">The expected outcome: true for success, false for failure, null for undetermined.</param>
    [Theory]
    [InlineData(ServerStability.Functional, true)]
    [InlineData(ServerStability.Flaky, null)]
    [InlineData(ServerStability.Failing, false)]
    public async Task ControllerGetPaymentGenerator_AnyHeader_ServerLogsSuccess(
        ServerStability serverStability,
        bool? expectSuccess)
    {
        // Arrange
        var hostname = _fixtures.ClientFixture.Hostname;
        var port = _fixtures.ClientFixture.Port;

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri($"http://{hostname}:{port}"),
        };

        // Add a custom header to control the Server's stability mode
        httpClient.DefaultRequestHeaders.Add(nameof(ServerStability), serverStability.ToString());

        using var scope = _fixtures.Factory.Services.CreateScope();

        // Get the expected log messages based on the stability mode
        var validLogMessages = ServerFixture.GetValidLogMessages(expectSuccess);

        // Set up a flag to track whether the consumer logs a message
        var consumerLogged = false;

        // Keep track of whether the consumer has logged the message of interest
        _fixtures.Factory.SetLogAction(
            condition: logMsg => logMsg.StartsWith("Payment"),
            action: () => consumerLogged = true);

        // Act
        var response = await httpClient.GetAsync("/PaymentGenerator");
        _output.WriteLine($"Response: {await response.Content.ReadAsStringAsync()}");

        // Wait for the consumer to log a message
        SpinWait.SpinUntil(() => consumerLogged, TimeSpan.FromSeconds(10));

        // Assert
        // Verify that the Server logged the expected messages
        _fixtures.Factory.Logger
            .Received().CallToLog(
                Arg.Any<LogLevel>(),
                verifier =>
                {
                    return Array.Exists(
                        validLogMessages,
                        validLogMessage => verifier.OriginalFormat.StartsWith(validLogMessage));
                });
    }
}
