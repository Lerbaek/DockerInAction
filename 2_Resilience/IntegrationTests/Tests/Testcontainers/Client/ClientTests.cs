using AutoFixture;
using IntegrationTests.Configuration.Collections;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests.Tests.Testcontainers.Client;

[Collection(nameof(ClientTestcontainersCollection))]
public class ClientTests
{
    private readonly ITestOutputHelper _output;
    private readonly ClientTestFixtures _fixtures;

    public ClientTests(ITestOutputHelper output,
        ClientTestFixtures fixtures)
    {
        _output = output;
        _fixtures = fixtures;
        
        fixtures.Factory.RabbitMqOptions.Port = fixtures.RabbitMqFixture.Port;
        fixtures.Factory.RabbitMqOptions.Host = fixtures.RabbitMqFixture.Hostname;
    }

    [Theory]
    [InlineData(ServerStability.Functional, true)]
    [InlineData(ServerStability.Failing, false)]
    public async Task HttpGetPaymentGenerator_AnyHeader_ServerLogsSuccess(ServerStability serverStability, bool expectSuccess)
    {
        // Arrange
        var httpClient = _fixtures.Factory.CreateClient();
        httpClient.DefaultRequestHeaders.Add(nameof(ServerStability), serverStability.ToString());
        var startTime = DateTime.Now;
        var initialLogLength = await _fixtures.ServerFixture.GetLogLength(startTime);

        // Act
        await httpClient.GetAsync("/PaymentGenerator");

        // Assert
        _fixtures.ServerFixture.AssertServerLogSuccess(expectSuccess, startTime, initialLogLength, _output);
    }

    [Theory]
    [InlineData(ServerStability.Functional, true)]
    [InlineData(ServerStability.Failing, false)]
    public async Task ControllerGetPaymentGenerator_AnyHeader_ServerLogsSuccess(ServerStability serverStability, bool expectSuccess)
    {
        // Arrange
        var startTime = DateTime.Now;
        var initialLogLength = await _fixtures.ServerFixture.GetLogLength(startTime);

        using var scope = _fixtures.Factory.Services.CreateScope();
        var publish = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
        
        // Act
        var payment = new Fixture().Create<Payment>();
        await publish.Publish(payment, context => context.Headers.Set(nameof(ServerStability), $"{serverStability}"));

        // Assert
        _fixtures.ServerFixture.AssertServerLogSuccess(expectSuccess, startTime, initialLogLength, _output);
    }
}