namespace BenchmarkDotNetVisualizer.Utilities;

/// <summary>
/// Color Helper
/// </summary>
public class ColorHelper
{
    /// <summary>
    /// Returns hex color value between Red (FF0000) and Green(00FF00 ) from a Scalar value between 0 and 1
    /// </summary>
    /// <param name="value">Scalar value between 0 and 1.</param>
    /// <returns>Hex color value</returns>
    public static string GetColorBetweenRedAndGreen(double value)
    {
        Guard.ThrowIfNotInRange(value, 0.0, 1.0, paramName: nameof(value));

        value *= 510;// value must be between [0, 510]

        double redValue;
        double greenValue;
        if (value < 255)
        {
            redValue = 255;
            greenValue = (Math.Sqrt(value) * 16);
        }
        else
        {
            greenValue = 255;
            value -= 255;
            redValue = 255 - (value * value / 255);
        }

        return "#" + ConvertToHex(redValue) + ConvertToHex(greenValue) + "00";

        static string ConvertToHex(double i) => ((int)i).ToString("X2");
    }

    /// <summary>
    /// Lightens the specified color.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <param name="amount">The amount.</param>
    /// <returns></returns>
    public static string Lighten(string color, double amount = 0.5)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(color, nameof(color));
        Guard.ThrowIfNotInRange(amount, 0.0, 1.0, paramName: nameof(amount));

        return MixColors(color, "#FFF", amount);
    }

    /// <summary>
    /// Darkens the specified color.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <param name="amount">The amount.</param>
    /// <returns></returns>
    public static string Darken(string color, double amount = 0.5)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(color, nameof(color));
        Guard.ThrowIfNotInRange(amount, 0.0, 1.0, paramName: nameof(amount));

        return MixColors(color, "#000", amount);
    }

    /// <summary>
    /// Mixes the colors.
    /// </summary>
    /// <param name="colorA">The color a.</param>
    /// <param name="colorB">The color b.</param>
    /// <param name="amount">The amount.</param>
    /// <returns></returns>
    public static string MixColors(string colorA, string colorB, double amount = 0.5)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(colorA, nameof(colorA));
        ArgumentException.ThrowIfNullOrWhiteSpace(colorB, nameof(colorB));
        Guard.ThrowIfNotInRange(amount, 0.0, 1.0, paramName: nameof(amount));

        var rgbA = HexToRGB(colorA);
        var rgbB = HexToRGB(colorB);

        var mixedColor = rgbA.Select((_, index) => MixChannels(rgbA[index], rgbB[index], amount)).ToArray();

        return "#" + string.Concat(mixedColor.Select(p => p.ToString("X2")));
    }

    /// <summary>
    /// Converts hexadecimal color to RGB color.
    /// </summary>
    /// <param name="hexColor">Color of the hexadecimal.</param>
    /// <returns></returns>
    public static int[] HexToRGB(string hexColor)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hexColor, nameof(hexColor));

        return GetChannels(hexColor).Select(p => Convert.ToInt32(p, 16)).ToArray();
    }

    /// <summary>
    /// Gets the channels of a hexadecimal color.
    /// </summary>
    /// <param name="hexColor">Color of the hexadecimal.</param>
    /// <returns></returns>
    public static string[] GetChannels(string hexColor)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hexColor, nameof(hexColor));

        if (hexColor[0] is '#')
            hexColor = hexColor[1..];

        return (hexColor.Length) switch
        {
            3 => hexColor.Chunk(1).Select(p => new string(p[0], 2)).ToArray(),
            6 => hexColor.Chunk(2).Select(p => new string(p)).ToArray(),
            _ => throw new System.ArgumentException($"Invalid hex color '{hexColor}'", nameof(hexColor))
        };
    }

    /// <summary>
    /// Mixes two hexadecimal channels.
    /// </summary>
    /// <param name="channelA">The channel a.</param>
    /// <param name="channelB">The channel b.</param>
    /// <param name="amount">The amount.</param>
    /// <returns></returns>
    public static int MixChannels(int channelA, int channelB, double amount)
    {
        Guard.ThrowIfNotInRange(amount, 0.0, 1.0, paramName: nameof(amount));

        var a = channelA * (1 - amount);
        var b = channelB * amount;
        return Convert.ToInt32(a + b);
    }
}