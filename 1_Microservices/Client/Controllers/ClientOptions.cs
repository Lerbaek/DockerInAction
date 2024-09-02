using System.ComponentModel.DataAnnotations;

namespace Server.Controllers;

public class ClientOptions
{
    [Required]
    public required Uri ServerPath { get; init; }
}