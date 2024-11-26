using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using AutoFixture;
using Client.Controllers;
using Docker.DotNet.Models;
using FluentAssertions;
using IntegrationTests.Configuration.Collections;
using IntegrationTests.Configuration.Factories;
using IntegrationTests.Configuration.Fixtures;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Shared;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests.Tests.Testcontainers.Client;

[Collection(nameof(ClientTestcontainersCollection))]
public class ClientTests
{
    private readonly ServerFixture<ClientTests> _serverFixture;

    private readonly ITestOutputHelper _output;
    private readonly ClientIntegrationTestWebAppFactory _factory;

    public ClientTests(ITestOutputHelper output,
        ClientIntegrationTestWebAppFactory factory,
        RabbitMqFixture<ClientTests> rabbitMqFixture,
        ServerFixture<ClientTests> serverFixture)
    {
        _output = output;
        _factory = factory;
        _serverFixture = serverFixture;

        _factory.RabbitMqOptions.Port = rabbitMqFixture.Port;
        _factory.RabbitMqOptions.Host = rabbitMqFixture.Hostname;
    }

    [Theory]
    [InlineData(ServerStability.Functional, true)]
    [InlineData(ServerStability.Failing, false)]
    public async Task HttpGetPaymentGenerator_AnyHeader_ServerLogsSuccess(ServerStability serverStability, bool expectSuccess)
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        httpClient.DefaultRequestHeaders.Add(nameof(ServerStability), serverStability.ToString());
        var startTime = DateTime.Now;
        var initialLogLength = await _serverFixture.GetLogLength(startTime);

        // Act
        var response = await httpClient.GetAsync("/PaymentGenerator");

        // Assert
        _serverFixture.AssertServerLogSuccess(expectSuccess, startTime, initialLogLength, _output);
    }

    [Theory]
    [InlineData(ServerStability.Functional, true)]
    [InlineData(ServerStability.Failing, false)]
    public async Task ControllerGetPaymentGenerator_AnyHeader_ServerLogsSuccess(ServerStability serverStability, bool expectSuccess)
    {
        // Arrange
        var startTime = DateTime.Now;
        var initialLogLength = await _serverFixture.GetLogLength(startTime);

        using var scope = _factory.Services.CreateScope();
        var publish = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
        
        // Act
        var payment = new Fixture().Create<Payment>();
        await publish.Publish(payment, context => context.Headers.Set(nameof(ServerStability), $"{serverStability}"));

        // Assert
        _serverFixture.AssertServerLogSuccess(expectSuccess, startTime, initialLogLength, _output);
    }
}