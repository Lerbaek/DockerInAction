using FluentAssertions;
using IntegrationTests.Configuration.Collections;
using IntegrationTests.Configuration.Factories;
using IntegrationTests.Configuration.Fixtures;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Shared;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests.Tests.Testcontainers.EndToEnd;

/// <summary>
/// End-to-end test the entire solution using dynamically created images containers for the <see cref="Client"/>> and <see cref="Server"/>> services.
/// </summary>
[Collection(nameof(EndToEndTestcontainersCollection))]
public class EndToEndTests(
    ITestOutputHelper output,
    //NetworkFixture<EndToEndTests> networkFixture,
    RabbitMqFixture<EndToEndTests> rabbitMqFixture,
    ServerFixture<EndToEndTests> serverFixture,
    ClientFixture<EndToEndTests> clientFixture)// : IAsyncLifetime
{
    [Theory]
    [InlineData(ServerStability.Functional, true)]
    [InlineData(ServerStability.Failing, false)]
    public async Task HttpGetPaymentGenerator_GetWithoutHeaders_ServerLogsSuccess(ServerStability serverStability, bool expectSuccess)
    {
        // Arrange
        var hostname = clientFixture.Hostname;
        var port = clientFixture.Port;

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri($"http://{hostname}:{port}"),
        };
        httpClient.DefaultRequestHeaders.Add(nameof(ServerStability), serverStability.ToString());

        var startTime = DateTime.Now;
        var initialLogLength = await serverFixture.GetLogLength(startTime);

        // Act
        var response = await httpClient.GetAsync("/PaymentGenerator");

        output.WriteLine($"Response: {await response.Content.ReadAsStringAsync()}");

        // Assert
        serverFixture.AssertServerLogSuccess(expectSuccess, startTime, initialLogLength, output);
    }

    ///// <inheritdoc/>
    //public async Task InitializeAsync() => await NetworkFixture<EndToEndTests>.Instance.InitializeAsync();

    ///// <inheritdoc/>
    //public async Task DisposeAsync() => await NetworkFixture<EndToEndTests>.Instance.DisposeAsync();
}