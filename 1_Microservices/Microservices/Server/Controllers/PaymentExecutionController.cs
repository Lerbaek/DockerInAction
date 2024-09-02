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
    //[UnstableHeader]
    public IActionResult Post(
        [FromHeader(Name = "UnstableServer")] bool shouldFailIntermittently,
        Payment payment)
    {
        var paymentJson = JsonSerializer.Serialize(payment, JsonSerializerOptions);
        if (shouldFailIntermittently && DateTime.Now.Ticks % 2 == 0)
        {
            logger.LogError("Payment failed: {Payment}", paymentJson);
            return StatusCode(500);
        }

        logger.LogInformation("Payment succeeded: {Payment}", paymentJson);
        return Ok(paymentJson);
    }
}