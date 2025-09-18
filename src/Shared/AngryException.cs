namespace Shared;

/// <summary>
/// An exception that represents a random angry reaction with a piece of ASCII art.
/// </summary>
public class AngryException() : Exception(GetAngryArt())
{
    private static readonly Random Random = new((int)(DateTime.Now.Ticks % int.MaxValue));

    private static readonly string[] AngryArts =
    [
        "(\u256f\u00b0\u25a1\u00b0)\u256f \u253b\u2501\u253b",
        "\u10DA(\u0CA0\u76CA\u0CA0)\u10DA",
        "(\u22DF\uFE4F\u22DE)",
        "\u00AF\\_(\u30C4)_/\u00AF",
        "(\u2565\uFE4F\u2565)",
        "\u0F3C \u3064 \u25D5_\u25D5 \u0F3D\u3064",
        "\u0B67\u0F3C\u0CA0\u76CA\u0CA0\u0F3D\u0B68"
    ];

    private static string GetAngryArt()
    {
        var index = Random.Next(AngryArts.Length);
        return AngryArts[index];
    }
}