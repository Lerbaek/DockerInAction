using System.Text.Json;
using MassTransit;
using Shared;

namespace Server;

/// <summary>
/// Consumer for processing payment messages received from the Client application.
/// <para>
/// This consumer demonstrates different response behaviors based on the ServerStability
/// header that accompanies each message, which is useful for testing error handling
/// and resilience scenarios in integration tests.
/// </para>
/// </summary>
/// <remarks>
/// The consumer supports three stability modes:
/// <list type="bullet">
/// <item>Functional - Always processes payments successfully</item>
/// <item>Flaky - Randomly succeeds or fails (50% chance based on system clock ticks)</item>
/// <item>Failing - Always fails with an exception</item>
/// </list>
/// These modes enable integration tests to verify system behavior under different conditions.
/// </remarks>
public class PaymentConsumer(ILogger<PaymentConsumer> logger) : IConsumer<Payment>
{
    /// <summary>
    /// Processes an incoming payment message from the message bus.
    /// </summary>
    /// <param name="context">The consume context containing the payment message and headers.</param>
    /// <returns>A completed task when processing finishes or an exception for failure scenarios.</returns>
    /// <exception cref="Exception">
    /// Thrown when:
    /// <list type="bullet">
    /// <item>The ServerStability header is set to Failing</item>
    /// <item>The ServerStability header is set to Flaky and the current tick count is even</item>
    /// </list>
    /// </exception>
    /// <remarks>
    /// This method:
    /// <list type="number">
    /// <item>Extracts the ServerStability setting from message headers</item>
    /// <item>Serializes the payment for logging</item>
    /// <item>Either logs success or throws an exception based on the stability setting</item>
    /// </list>
    /// </remarks>
    public Task Consume(ConsumeContext<Payment> context)
    {
        Enum.TryParse(
            typeof(ServerStability),
            context.GetHeader(nameof(ServerStability)),
            false,
            out var serverStability);

        var payment = context.Message;
        var paymentJson = JsonSerializer.Serialize(payment, Configuration.Options.JsonSerializerOptions);

        if (serverStability is ServerStability.Flaky && DateTime.Now.Ticks % 2 == 0
            || serverStability is ServerStability.Failing)
        {
            logger.LogError("Payment failed: {Payment}", paymentJson);
#pragma warning disable S112
            throw new Exception("(\u256f\u00b0\u25a1\u00b0)\u256f \u253b\u2501\u253b");
#pragma warning restore S112
        }

        logger.LogInformation("Payment succeeded: {Payment}", paymentJson);
        return Task.CompletedTask;
    }
}
