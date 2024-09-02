using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace Server.Controllers;

[ApiController]
[Route("[controller]")]
public class PaymentExecutionController(ILogger<PaymentExecutionController> logger) : ControllerBase
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true
    };

    [HttpPost]
    public IActionResult Post(
        Payment payment,
        [FromHeader(Name = nameof(ServerStability))] ServerStability serverStability = ServerStability.Functional)
    {
        var paymentJson = JsonSerializer.Serialize(payment, JsonSerializerOptions);
        if (serverStability is ServerStability.Flaky && DateTime.Now.Ticks % 2 == 0
            || serverStability is ServerStability.Failing)
        {
            logger.LogError("Payment failed: {Payment}", paymentJson);
            return StatusCode(500);
        }

        logger.LogInformation("Payment succeeded: {Payment}", paymentJson);
        return Ok(paymentJson);
    }
}