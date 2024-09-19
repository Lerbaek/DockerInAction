using System.Text.Json.Serialization;
using Client.Configuration;
using Client.Controllers;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddHttpClient<PaymentGeneratorController>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOptions<ClientOptions>().BindConfiguration("ClientOptions");
builder.Services.AddOptions<RabbitMqTransportOptions>().BindConfiguration("RabbitMq");

builder.Services.AddMassTransit(configurator =>
{
    //var options = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqOptions>();
    configurator.UsingRabbitMq(//(context, cfg) =>
    //{
    //    //cfg.Host(options.Host, 5672, "/", c =>
    //    //{
    //    //    c.Username(options.User);
    //    //    c.Password(options.Password);
    //    //});
    //}
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

await app.RunAsync();
