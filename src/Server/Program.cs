using System.Text.Json.Serialization;
using MassTransit;
using Server.Configuration;

namespace Server;

public class Program
{
    protected Program() {}

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddOptions<RabbitMqTransportOptions>().BindConfiguration("RabbitMq");

        builder.Services.AddMassTransit(configurator =>
        {
            configurator.AddConsumer<PaymentConsumer>();

            configurator.AddConfigureEndpointsCallback((name, cfg) =>
            {
                if (cfg is IRabbitMqReceiveEndpointConfigurator rabbitMqConfigurator)
                {
                    rabbitMqConfigurator.SetQuorumQueue();
                }

                cfg.UseMessageRetry(r => r.Exponential(
                    retryLimit: builder
                        .Configuration
                        .GetSection("RabbitMq")
                        .Get<RabbitMqOptions>()?
                        .Retries ?? 10,
                    minInterval: TimeSpan.Zero,
                    maxInterval: TimeSpan.FromSeconds(2),
                    intervalDelta: TimeSpan.FromMilliseconds(500)));
            });

            configurator.UsingRabbitMq((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}