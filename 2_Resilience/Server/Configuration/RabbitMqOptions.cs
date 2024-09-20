using System.ComponentModel.DataAnnotations;

namespace Server.Configuration;

public class RabbitMqOptions
{
    [Required]
    public required string Host { get; init; }
    [Required]
    public required string Username { get; init; }
    [Required]
    public required string Password { get; init; }
    [Required]
    public required int Retries { get; init; }
}
