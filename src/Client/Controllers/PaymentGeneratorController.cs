using AutoFixture;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared;
using static System.Text.Json.JsonSerializer;

namespace Client.Controllers;

/// <summary>
/// Controller for generating and publishing payment messages.
/// <para>
/// This controller serves as a demonstration endpoint for integration testing with Testcontainers.
/// It generates a random payment and publishes it to the message bus, which is then processed
/// by the Server application. The behavior can be controlled via the ServerStability header.
/// </para>
/// </summary>
/// <remarks>
/// This simple controller is specifically designed to demonstrate various integration test approaches:
/// <list type="bullet">
/// <item>End-to-end testing with all components in containers</item>
/// <item>Hybrid testing with the Client in-process and containerized Server</item>
/// <item>Hybrid testing with the Server in-process and containerized Client</item>
/// </list>
/// </remarks>
[ApiController]
[Route("[controller]")]
public class PaymentGeneratorController(
    ILogger<PaymentGeneratorController> logger,
    IPublishEndpoint publish) : ControllerBase
{
    private readonly Fixture _fixture = new();

    /// <summary>
    /// Generates a random payment and publishes it to the message bus.
    /// </summary>
    /// <param name="serverStability">Controls how the Server responds to the payment:
    /// <list type="bullet">
    /// <item>Functional - Server processes the payment successfully</item>
    /// <item>Flaky - Server randomly succeeds or fails (50% chance)</item>
    /// <item>Failing - Server always fails with an exception</item>
    /// </list>
    /// </param>
    /// <returns>The generated payment object</returns>
    /// <remarks>
    /// This endpoint demonstrates:
    /// <list type="number">
    /// <item>Using AutoFixture to generate test data</item>
    /// <item>Publishing messages with MassTransit</item>
    /// <item>Setting message headers for controlling test behavior</item>
    /// <item>Returning the generated data for verification in tests</item>
    /// </list>
    /// </remarks>
    [HttpGet]
    public async Task<IActionResult> Get([FromHeader(Name = nameof(ServerStability))] ServerStability serverStability = ServerStability.Functional)
    {
        var payment = _fixture.Create<Payment>();
        logger.LogInformation("Sending payment: {Payment}", Serialize(payment));
        await publish.Publish(payment, context => context.Headers.Set(nameof(ServerStability), $"{serverStability}"));
        return Ok(payment);
    }
}