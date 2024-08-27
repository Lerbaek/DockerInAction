using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentExecutionController(ILogger<PaymentExecutionController> logger) : ControllerBase
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            WriteIndented = true
        };

        [HttpPost]
        public IActionResult Post(Payment payment)
        {
            var paymentJson = JsonSerializer.Serialize(payment, JsonSerializerOptions);
            logger.LogInformation("Received payment: {Payment}", paymentJson);
            return Ok(paymentJson);
        }
    }
}