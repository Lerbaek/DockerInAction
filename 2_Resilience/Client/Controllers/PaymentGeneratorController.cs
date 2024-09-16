using System.Text;
using AutoFixture;
using Client.Configuration;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shared;

namespace Client.Controllers;

[ApiController]
[Route("[controller]")]
public class PaymentGeneratorController(
    HttpClient httpClient,
    IOptions<ClientOptions> options,
    ILogger<PaymentGeneratorController> logger,
    IPublishEndpoint publish) : ControllerBase
{
    private readonly Fixture _fixture = new();

    [HttpGet]
    public async Task<IActionResult> Get([FromHeader(Name = nameof(ServerStability))] ServerStability serverStability = ServerStability.Functional)
    {
        var payment = _fixture.Create<Payment>();
        await publish.Publish(payment);
        return Ok();
    }
}