using FluentAssertions;
using IntegrationTests.Configuration.Collections;
using IntegrationTests.Configuration.Factories;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using Xunit;

namespace IntegrationTests.ClientTests;

[Collection(nameof(ClientIntegrationTestWebAppFactoryCollection))]
public class PaymentGeneratorControllerTests(ClientIntegrationTestWebAppFactory factory)
{
    private readonly ITestHarness _testHarness = factory.Services.GetRequiredService<ITestHarness>();
    private readonly HttpClient _httpClient = factory.CreateClient();

    [Fact]
    public async Task Get_RequestIsReceived_PaymentIsPublished()
    {
        // Arrange
        var timeoutSeconds = 10;
        var consumed = false;

        // Act
        await _testHarness.Start();
        var response = await _httpClient.GetAsync("/PaymentGenerator");
        
        // Assert
        response.Invoking(r => r.EnsureSuccessStatusCode()).Should().NotThrow();

        // Set a timeout for the consumption check
        while(timeoutSeconds --> 0)
        {
            consumed = await _testHarness.Published.Any<Payment>();

            if (!consumed)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        await _testHarness.Stop();

        consumed.Should().BeTrue();
    }
}