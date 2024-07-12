using System.ComponentModel;

namespace BenchmarkDotNetVisualizer;

/// <summary>
/// Image Format
/// </summary>
public enum ImageFormat
{
    /// <summary>
    /// The PNG format
    /// </summary>
    Png,
    /// <summary>
    /// The JPEG format
    /// </summary>
    Jpeg,
    /// <summary>
    /// The Webp format
    /// </summary>
    Webp
}

/// <summary>
/// ImageFormat Extensions
/// </summary>
public static class ImageFormatExtensions
{
    /// <summary>
    /// Gets extension based on image format.
    /// </summary>
    /// <param name="format">The image format.</param>
    /// <returns></returns>
    public static string GetExtension(this ImageFormat format)
    {
        return format switch
        {
            ImageFormat.Png => "png",
            ImageFormat.Jpeg => "jpg",
            ImageFormat.Webp => "webp",
            _ => throw new InvalidEnumArgumentException(nameof(format), Convert.ToInt32(format), format.GetType())
        };
    }
}