using AutoFixture;
using FluentAssertions;
using IntegrationTests.Configuration.Collections;
using IntegrationTests.Configuration.Factories;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using Xunit;

namespace IntegrationTests.Tests.TestHarness.Server;

/// <summary>
/// Unit tests for the Server application using MassTransit's TestHarness.
/// <para>
/// These tests use an in-memory approach where:
/// </para>
/// <list type="bullet">
/// <item>The Server application runs in-process via WebApplicationFactory</item>
/// <item>MassTransit's TestHarness provides an in-memory message broker</item>
/// <item>No Docker containers are used, making tests faster and more isolated</item>
/// </list>
/// <para>
/// This approach is ideal for testing the Server's consumption behavior without the overhead
/// of containerization, focusing specifically on message handling logic.
/// </para>
/// </summary>
[Collection(nameof(ServerTestHarnessCollection))]
public class ServerTests : IAsyncDisposable
{
    private readonly Fixture _fixture = new();
    private readonly ITestHarness _testHarness;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerTests"/> class and configures the test harness.
    /// </summary>
    /// <param name="factory">The factory for creating a server application for testing.</param>
    public ServerTests(ServerIntegrationTestWebAppFactory factory)
    {
        // Configure the factory to use MassTransit's test harness
        factory.UseTestHarness();
        _testHarness = factory.Services.GetRequiredService<ITestHarness>();
        _testHarness.Start();
    }

    /// <summary>
    /// Tests that the Server correctly consumes Payment messages and responds according to stability settings.
    /// <para>
    /// The test verifies that:
    /// </para>
    /// <list type="bullet">
    /// <item>The Server successfully consumes Payment messages</item>
    /// <item>The Server correctly interprets the stability header</item>
    /// <item>In Functional mode, processing completes without exceptions</item>
    /// <item>In Failing mode, an exception is thrown as expected</item>
    /// </list>
    /// </summary>
    /// <param name="serverStability">Controls the Server's behavior: Functional (success) or Failing (error).</param>
    /// <param name="exceptionExpected">Whether an exception is expected during message processing.</param>
    [Theory]
    [InlineData(ServerStability.Functional, false)]
    [InlineData(ServerStability.Failing, true)]
    public async Task Consume_PaymentIsConsumed_ObeysStabilityInstruction(ServerStability serverStability, bool exceptionExpected)
    {
        // Arrange
        var payment = _fixture.Create<Payment>();
        var startTime = DateTime.Now.ToUniversalTime();

        // Act
        await _testHarness.Bus.Publish(
            payment,
            context => context.Headers.Set(nameof(ServerStability), $"{serverStability}"));

        // Assert
        var success = await _testHarness.Consumed.Any<Payment>(
            context =>
                // Only consider at messages newer than startTime
                context.StartTime.ToUniversalTime() > startTime
                &&
                exceptionExpected == context.Exception is { });

        success.Should().BeTrue();
    }

    /// <summary>
    /// Stops the test harness and disposes resources when the test finishes.
    /// </summary>
    public async ValueTask DisposeAsync() => await _testHarness.Stop();
}
