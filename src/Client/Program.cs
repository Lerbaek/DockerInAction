using System.Text.Json.Serialization;
using Client.Configuration;
using Client.Controllers;
using MassTransit;

namespace Client;

public class Program
{
    protected Program() {}

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddHttpClient<PaymentGeneratorController>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddOptions<ClientOptions>().BindConfiguration("ClientOptions");

        //builder.Services.AddMassTransit(configurator =>
        //{
        //    configurator.DisableUsageTelemetry();

        //    var options = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqTransportOptions>();
        //    if (options is null) return;

        //    configurator.UsingRabbitMq((context, cfg) =>
        //        cfg.Host(options.Host, options.Port, "/", c =>
        //        {
        //            c.Username(options.User);
        //            c.Password(options.Pass);
        //        }));
        //});

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync();
    }
}