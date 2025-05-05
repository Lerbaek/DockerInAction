using AutoFixture;
using IntegrationTests.Configuration.Collections;
using IntegrationTests.Tests.Testcontainers.Fixtures;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests.Tests.Testcontainers.Client;

/// <summary>
/// Integration tests for the Client application using Testcontainers.
/// <para>
/// These tests use a hybrid approach where:
/// </para>
/// <list type="bullet">
/// <item>The Client application runs in-process via WebApplicationFactory</item>
/// <item>The Server application runs in a Docker container</item>
/// <item>RabbitMQ runs in a Docker container</item>
/// </list>
/// <para>
/// This approach offers faster test execution and easier debugging compared to full end-to-end 
/// container tests, while still testing integration with containerized dependencies.
/// </para>
/// </summary>
/// <param name="output">Output helper for writing to test logs.</param>
/// <param name="fixtures">Test fixtures providing access to test dependencies.</param>
[Collection(nameof(ClientTestcontainersCollection))]
public class ClientTests(
    ITestOutputHelper output,
    ClientTestFixtures fixtures)
{
    /// <summary>
    /// Tests that HTTP GET requests to the PaymentGenerator endpoint correctly interact with the Server.
    /// <para>
    /// The test verifies that:
    /// </para>
    /// <list type="bullet">
    /// <item>The Client application successfully handles HTTP requests</item>
    /// <item>The Client forwards the requests to the Server via RabbitMQ</item>
    /// <item>The Server processes the payment according to the specified stability mode</item>
    /// <item>The Server logs the expected success/failure messages</item>
    /// </list>
    /// </summary>
    /// <param name="serverStability">Controls the Server's behavior: Functional (success), Flaky (inconsistent), or Failing (error).</param>
    /// <param name="expectSuccess">The expected outcome: true for success, false for failure, null for undetermined.</param>
    [Theory]
    [InlineData(ServerStability.Functional, true)]
    [InlineData(ServerStability.Flaky, null)]
    [InlineData(ServerStability.Failing, false)]
    public async Task HttpGetPaymentGenerator_AnyHeader_ServerLogsSuccess(ServerStability serverStability, bool? expectSuccess)
    {
        // Arrange: Create an HTTP client and set up headers for the request.
        // Useful methods: fixtures.Factory.CreateClient(), httpClient.DefaultRequestHeaders.Add()

        // Act: Send an HTTP GET request to the PaymentGenerator endpoint.
        // Useful methods: httpClient.GetAsync("/PaymentGenerator")

        // Assert: Verify that the server logs contain the expected messages.
        // Useful methods: fixtures.ServerFixture.AssertServerLogSuccess()
    }

    /// <summary>
    /// Tests that direct message publishing via MassTransit correctly integrates with the Server.
    /// <para>
    /// This test bypasses the HTTP layer and directly tests the messaging integration by:
    /// </para>
    /// <list type="bullet">
    /// <item>Publishing a Payment message directly to the message bus registered in the Client</item>
    /// <item>Verifying that the Server receives and processes the message</item>
    /// <item>Checking that the Server logs the expected success/failure messages</item>
    /// </list>
    /// <para>
    /// This approach isolates the messaging integration from HTTP concerns.
    /// </para>
    /// </summary>
    /// <param name="serverStability">Controls the Server's behavior: Functional (success), Flaky (inconsistent), or Failing (error).</param>
    /// <param name="expectSuccess">The expected outcome: true for success or false for failure.</param>
    [Theory]
    [InlineData(ServerStability.Functional, true)]
    [InlineData(ServerStability.Flaky, null)]
    [InlineData(ServerStability.Failing, false)]
    public async Task ControllerGetPaymentGenerator_AnyHeader_ServerLogsSuccess(ServerStability serverStability, bool? expectSuccess)
    {
        // Arrange: Set up the test environment, including the message bus and log tracking.
        // Useful methods: fixtures.Factory.Services.CreateScope(), fixtures.ServerFixture.GetLogLength()

        // Act: Publish a Payment message directly to the message bus.
        // Useful methods: publish.Publish()

        // Assert: Verify that the server logs contain the expected messages.
        // Useful methods: fixtures.ServerFixture.AssertServerLogSuccess()
    }
}
