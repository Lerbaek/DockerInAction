using System.Text.Json.Serialization;
using MassTransit;
using MassTransit.Configuration;
using Shared;

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
    configurator.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    }
    );
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

public class PaymentConsumer(ILogger<PaymentConsumer> logger) : IConsumer<Payment>
{
    public Task Consume(ConsumeContext<Payment> context)
    {
        logger.LogInformation("This worked. Next, make it sometimes fail");
        return Task.CompletedTask;
    }
}
