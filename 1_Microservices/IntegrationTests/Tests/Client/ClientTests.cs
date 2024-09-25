using FluentAssertions;
using IntegrationTests.Configuration.Collections;
using IntegrationTests.Configuration.Factories;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using Xunit;

namespace IntegrationTests.Tests.Client;

[Collection(nameof(ClientIntegrationTestWebAppFactoryCollection))]
public class ClientTests : IAsyncDisposable
{
    private readonly ITestHarness _testHarness;
    private readonly HttpClient _httpClient;

    public ClientTests(ClientIntegrationTestWebAppFactory factory)
    {
        _testHarness = factory.Services.GetRequiredService<ITestHarness>();
        _httpClient = factory.CreateClient();
        _testHarness.Start();
    }

    [Fact]
    public async Task Get_RequestIsReceived_PaymentIsPublished()
    {
        // Arrange

        // Act
        var response = await _httpClient.GetAsync("/PaymentGenerator");

        // Assert
        response.Invoking(r => r.EnsureSuccessStatusCode()).Should().NotThrow();
        var consumed = await _testHarness.Published.Any<Payment>();
        consumed.Should().BeTrue();
    }

    public async ValueTask DisposeAsync() => await _testHarness.Stop();
}