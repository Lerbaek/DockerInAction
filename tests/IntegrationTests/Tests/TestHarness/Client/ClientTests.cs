using FluentAssertions;
using IntegrationTests.Configuration.Collections;
using IntegrationTests.Configuration.Factories;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using Xunit;

namespace IntegrationTests.Tests.TestHarness.Client;

/// <summary>
/// Unit tests for the Client application using MassTransit's TestHarness.
/// <para>
/// These tests use an in-memory approach where:
/// </para>
/// <list type="bullet">
/// <item>The Client application runs in-process via WebApplicationFactory</item>
/// <item>MassTransit's TestHarness provides an in-memory message broker</item>
/// <item>No Docker containers are used, making tests faster and more isolated</item>
/// </list>
/// <para>
/// This approach is ideal for testing the Client's core functionality without the overhead
/// of containerization, allowing for rapid feedback during development.
/// </para>
/// </summary>
[Collection(nameof(ClientTestHarnessCollection))]
public class ClientTests : IAsyncDisposable
{
    private readonly ITestHarness _testHarness;
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientTests"/> class and configures the test harness.
    /// </summary>
    /// <param name="factory">The factory for creating a client application for testing.</param>
    public ClientTests(ClientIntegrationTestWebAppFactory factory)
    {
        // Configure the factory to use MassTransit's test harness
        factory.UseTestHarness();
        _testHarness = factory.Services.GetRequiredService<ITestHarness>();
        _httpClient = factory.CreateClient();
        _testHarness.Start();
    }

    /// <summary>
    /// Tests that HTTP GET requests to the PaymentGenerator endpoint result in a published message.
    /// <para>
    /// The test verifies that:
    /// </para>
    /// <list type="bullet">
    /// <item>The Client application successfully handles HTTP requests</item>
    /// <item>The request results in a Payment message being published</item>
    /// <item>The HTTP response is successful</item>
    /// </list>
    /// </summary>
    [Fact]
    public async Task Get_RequestIsReceived_PaymentIsPublished()
    {
        // Arrange: Set up the HTTP client and test harness for the test.
        // Useful methods: factory.UseTestHarness(), _testHarness.Start()

        // Act: Send an HTTP GET request to the PaymentGenerator endpoint.
        // Useful methods: _httpClient.GetAsync()

        // Assert: Verify that a Payment message was published and the HTTP response was successful.
        // Useful methods: _testHarness.Published.Any<Payment>(), response.EnsureSuccessStatusCode()
    }

    /// <summary>
    /// Stops the test harness and disposes resources when the test finishes.
    /// </summary>
    public async ValueTask DisposeAsync() => await _testHarness.Stop();
}
