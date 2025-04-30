using AutoFixture;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared;
using static System.Text.Json.JsonSerializer;

namespace Client.Controllers;

[ApiController]
[Route("[controller]")]
public class PaymentGeneratorController(
    ILogger<PaymentGeneratorController> logger,
    IPublishEndpoint publish) : ControllerBase
{
    private readonly Fixture _fixture = new();

    [HttpGet]
    public async Task<IActionResult> Get([FromHeader(Name = nameof(ServerStability))] ServerStability serverStability = ServerStability.Functional)
    {
        var payment = _fixture.Create<Payment>();
        logger.LogInformation("Sending payment: {Payment}", Serialize(payment));
        await publish.Publish(payment, context => context.Headers.Set(nameof(ServerStability), $"{serverStability}"));
        return Ok(payment);
    }
}