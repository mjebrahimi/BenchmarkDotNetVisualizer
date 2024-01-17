// Ignore Spelling: Jpeg Png Webp metadata

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Metadata;

namespace BenchmarkDotNetVisualizer.Utilities;

/// <summary>
/// Shims for backward compatibility to older versions of ImageSharp
/// </summary>
public static class ImageSharpShims
{
#if NET6_0_OR_GREATER
    /// <summary>
    /// Gets the decoded image format.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    /// <returns></returns>
    public static IImageFormat? GetDecodedImageFormat(this ImageMetadata metadata)
    {
        return metadata.DecodedImageFormat;
    }
#else
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
    private static readonly System.Reflection.FieldInfo? formatMetadataField = typeof(ImageMetadata).GetField("formatMetadata", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

    /// <summary>
    /// Gets the decoded image format.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    /// <returns></returns>
    public static IImageFormat? GetDecodedImageFormat(this ImageMetadata metadata)
    {
        var formatMetadataValue = formatMetadataField?.GetValue(metadata) as Dictionary<IImageFormat, IDeepCloneable>;
        return formatMetadataValue?.First().Key;
    }

    /// <summary>
    /// For the specified file extensions type find the e <see cref="IImageFormat"/>.
    /// </summary>
    /// <param name="formatManager">The format manager.</param>
    /// <param name="extension">The extension to return the format for.</param>
    /// <param name="format">
    /// When this method returns, contains the format that matches the given extension;
    /// otherwise, the default value for the type of the <paramref name="format"/> parameter.
    /// This parameter is passed uninitialized.
    /// </param>
    /// <returns><see langword="true"/> if a match is found; otherwise, <see langword="false"/></returns>
    public static bool TryFindFormatByFileExtension(this ImageFormatManager formatManager, string extension, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out IImageFormat? format)
    {
        if (!string.IsNullOrWhiteSpace(extension) && extension[0] == '.')
        {
            extension = extension[1..];
        }

        format = formatManager.ImageFormats.FirstOrDefault(x =>
            x.FileExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase));

        return format is not null;
    }
#endif
}