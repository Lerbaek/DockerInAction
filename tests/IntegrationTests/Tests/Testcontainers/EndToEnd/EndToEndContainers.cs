using IntegrationTests.Configuration.Collections;
using Microsoft.Extensions.Logging;
using Shared;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests.Tests.Testcontainers.EndToEnd;

/// <summary>
/// End-to-end test the entire solution using dynamically created images containers for the <see cref="Client"/>> and <see cref="Server"/>> services.
/// </summary>
/// <remarks>
/// End-to-end test the entire solution using dynamically created images containers for the <see cref="Client"/>> and <see cref="Server"/>> services.
/// </remarks>
[Collection(nameof(EndToEndTestcontainersCollection))]
public class EndToEndContainers(
    ITestOutputHelper output,
    EndToEndTestFixtures fixtures)
{
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