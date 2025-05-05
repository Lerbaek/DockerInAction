using AutoFixture;
using Client.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shared;
using System.Net.Http;
using System.Text;
using static System.Text.Json.JsonSerializer;

namespace Client.Controllers;

/// <summary>
/// Controller for generating and sending payment messages via HTTP.
/// <para>
/// This controller serves as a demonstration endpoint for integration testing with Testcontainers.
/// It generates a random payment and sends it to the Server application using HTTP POST requests.
/// The behavior can be controlled via the ServerStability header.
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
    HttpClient httpClient,
    ILogger<PaymentGeneratorController> logger,
    IOptions<ClientOptions> options) : ControllerBase
{
    private readonly Fixture _fixture = new();

    /// <summary>
    /// Generates a random payment and sends it to the Server application via HTTP POST.
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
    /// <item>Sending HTTP POST requests to the Server</item>
    /// <item>Setting HTTP headers for controlling test behavior</item>
    /// <item>Returning the generated data for verification in tests</item>
    /// </list>
    /// </remarks>
    [HttpGet]
    public async Task<IActionResult> Get([FromHeader(Name = nameof(ServerStability))] ServerStability serverStability = ServerStability.Functional)
    {
        var payment = _fixture.Create<Payment>();
        var paymentJson = Serialize(payment);

        var request = new HttpRequestMessage(HttpMethod.Post, options.Value.Path)
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