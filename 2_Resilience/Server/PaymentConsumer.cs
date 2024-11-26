using System.Text.Json;
using MassTransit;
using Shared;

namespace Server;

public class PaymentConsumer(ILogger<PaymentConsumer> logger) : IConsumer<Payment>
{
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