using System.Text.Json;

namespace Server.Configuration;

public static class Options
{
    public static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true
    };
}