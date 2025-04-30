﻿using AutoFixture;
using FluentAssertions;
using IntegrationTests.Configuration.Collections;
using IntegrationTests.Configuration.Factories;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using Xunit;

namespace IntegrationTests.Tests.TestHarness.Server;

[Collection(nameof(ServerTestHarnessCollection))]
public class ServerTests : IAsyncDisposable
{
    private readonly Fixture _fixture = new();
    private readonly ITestHarness _testHarness;

    public ServerTests(ServerIntegrationTestWebAppFactory factory)
    {
        factory.UseTestHarness();
        _testHarness = factory.Services.GetRequiredService<ITestHarness>();
        _testHarness.Start();
    }

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

    public async ValueTask DisposeAsync() => await _testHarness.Stop();
}