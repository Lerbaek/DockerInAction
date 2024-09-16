using System.ComponentModel.DataAnnotations;

namespace Client.Configuration;

public class ClientOptions
{
    [Required]
    public required Uri Path { get; init; }
}