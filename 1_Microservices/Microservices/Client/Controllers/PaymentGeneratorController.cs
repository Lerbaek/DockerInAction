using System.Text;
using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Server.Controllers;
using Shared;

namespace Client.Controllers;

[ApiController]
[Route("[controller]")]
public class PaymentGeneratorController(
    HttpClient httpClient,
    IOptions<ClientOptions> options,
    ILogger<PaymentGeneratorController> logger) : ControllerBase
{
    private readonly Fixture _fixture = new();

    [HttpGet]
    public async Task<IActionResult> Get([FromHeader(Name = nameof(ServerStability))] ServerStability serverStability = ServerStability.Functional)
    {
        var payment = _fixture.Create<Payment>();
        var paymentJson = System.Text.Json.JsonSerializer.Serialize(payment);

        var request = new HttpRequestMessage(HttpMethod.Post, options.Value.ServerPath)
        {
            Content = new StringContent(paymentJson, Encoding.UTF8, "application/json"),
        };

        request.Headers.Add(nameof(ServerStability), serverStability.ToString());

        logger.LogInformation("Sending payment: {Payment}", paymentJson);

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var responseJson = await response.Content.ReadAsStringAsync();
        return Ok(responseJson);
    }
}