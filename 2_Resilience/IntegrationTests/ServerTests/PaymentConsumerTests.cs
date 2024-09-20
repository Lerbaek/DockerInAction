using AutoFixture;
using FluentAssertions;
using IntegrationTests.Configuration.Factories;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using Xunit;

namespace IntegrationTests.ServerTests;

[Collection(nameof(ServerIntegrationTestWebAppFactoryCollection))]
public class PaymentConsumerTests(ServerIntegrationTestWebAppFactory factory)
{
    private readonly Fixture _fixture = new();

    [Theory]
    [InlineData(ServerStability.Functional)]
    [InlineData(ServerStability.Failing)]
    public async Task Consume_PaymentIsConsumed_ObeysStabilityInstruction(ServerStability serverStability)
    {
        // Arrange
        var testHarness = factory.Services.GetRequiredService<ITestHarness>();
        var timeoutSeconds = 10;

        // Act
        await testHarness.Start();
        
        // Assert
        await testHarness.Bus.Publish(_fixture.Create<Payment>(), context => context.Headers.Set(nameof(ServerStability), $"{serverStability}"));

        var startTime = DateTime.Now.AddMilliseconds(-100).ToUniversalTime();

        // Set a timeout for the consumption check
        while(timeoutSeconds --> 0)
        {
            var consumed = await testHarness.Consumed.Any<Payment>(context => context.StartTime.ToUniversalTime() > startTime);

            if (!consumed)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        var success = await testHarness.Consumed.Any<Payment>(
            context =>
                context.StartTime.ToUniversalTime() > startTime
                &&
                (serverStability == ServerStability.Failing
                    ? context.Exception is not null
                    : context.Exception is null));

        await testHarness.Stop();
        success.Should().BeTrue();
    }
}