using IntegrationTests.Configuration.Collections;
using IntegrationTests.Tests.Testcontainers.Fixtures;
using Shared;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests.Tests.Testcontainers.EndToEnd;


/// <summary>
/// End-to-end tests for the entire solution using Testcontainers.
/// <para>
/// These tests validate the interaction between the Client and Server applications
/// in a fully containerized environment. The setup includes:
/// </para>
/// <list type="bullet">
/// <item>The Client application running in a Docker container</item>
/// <item>The Server application running in a Docker container</item>
/// <item>RabbitMQ running in a Docker container</item>
/// </list>
/// <para>
/// This approach ensures that the entire system is tested as it would run in production,
/// including network communication, container orchestration, and service dependencies.
/// </para>
/// </summary>
[Collection(nameof(EndToEndTestcontainersCollection))]
public class EndToEndContainersTests(
    ITestOutputHelper output,
    EndToEndTestFixtures fixtures)
{
    /// <summary>
    /// Tests that HTTP GET requests to the PaymentGenerator endpoint are processed correctly
    /// in a fully containerized environment.
    /// <para>
    /// The test verifies that:
    /// </para>
    /// <list type="bullet">
    /// <item>The Client application successfully sends HTTP requests to the Server</item>
    /// <item>The Server processes the requests and interacts with RabbitMQ</item>
    /// <item>The Server logs the expected success or failure messages based on the specified stability mode</item>
    /// </list>
    /// </summary>
    /// <param name="serverStability">Controls the Server's behavior: Functional (success) or Failing (error).</param>
    /// <param name="expectSuccess">The expected outcome: true for success, false for failure.</param>
    [Theory]
    [InlineData(ServerStability.Functional, true)]
    [InlineData(ServerStability.Failing, false)]
    public async Task HttpGetPaymentGenerator_GetWithoutHeaders_ServerLogsSuccess(ServerStability serverStability, bool expectSuccess)
    {
        // Arrange
        var hostname = fixtures.ClientFixture.Hostname;
        var port = fixtures.ClientFixture.Port;

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri($"http://{hostname}:{port}"),
        };

        // Add a custom header to control the Server's stability mode
        httpClient.DefaultRequestHeaders.Add(nameof(ServerStability), serverStability.ToString());

        var startTime = DateTime.Now;
        var initialLogLength = await fixtures.ServerFixture.GetLogLength(startTime);

        // Act
        var response = await httpClient.GetAsync("/PaymentGenerator");

        output.WriteLine($"Response: {await response.Content.ReadAsStringAsync()}");

        // Assert
        fixtures.ServerFixture.AssertServerLogSuccess(expectSuccess, startTime, initialLogLength, output);
    }
}
