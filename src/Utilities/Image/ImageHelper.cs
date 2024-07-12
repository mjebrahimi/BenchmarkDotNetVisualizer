using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using System.ComponentModel;

namespace BenchmarkDotNetVisualizer.Utilities;

/// <summary>
/// Image Helper
/// </summary>
public static class ImageHelper
{
    #region CompressImageAndSaveAs
    /// <summary>
    /// Compresses the specified path.
    /// </summary>
    /// <param name="sourcePath">The path.</param>
    /// <param name="destinationPath">The path of destination image.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static async Task CompressImageAndSaveAsAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourcePath, nameof(sourcePath));
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationPath, nameof(destinationPath));

        var format = FindFormatByFileExtension(destinationPath);
        using var image = await Image.LoadAsync(sourcePath, cancellationToken);
        var encoder = format.GetImageEncoder();
        await image.SaveAsync(destinationPath, encoder, cancellationToken);
    }

    /// <summary>
    /// Compresses the specified path.
    /// </summary>
    /// <param name="sourcePath">The path of source image.</param>
    /// <param name="destinationPath">The path of destination image.</param>
    /// <param name="compressionLevel">The compression level.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static async Task CompressImageAndSaveAsAsync(string sourcePath, string destinationPath, ImageCompressionLevel compressionLevel, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourcePath, nameof(sourcePath));
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationPath, nameof(destinationPath));

        var format = FindFormatByFileExtension(destinationPath);
        using var image = await Image.LoadAsync(sourcePath, cancellationToken);
        var encoder = format.GetImageEncoder(compressionLevel);
        await image.SaveAsync(destinationPath, encoder, cancellationToken);
    }

    /// <summary>
    /// Compresses the specified path.
    /// </summary>
    /// <param name="sourcePath">The path.</param>
    /// <param name="destinationPath">The path of destination image.</param>
    public static void CompressImageAndSaveAs(string sourcePath, string destinationPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourcePath, nameof(sourcePath));
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationPath, nameof(destinationPath));

        var format = FindFormatByFileExtension(destinationPath);
        using var image = Image.Load(sourcePath);
        var encoder = format.GetImageEncoder();
        image.Save(destinationPath, encoder);
    }

    /// <summary>
    /// Compresses the specified path.
    /// </summary>
    /// <param name="sourcePath">The path of source image.</param>
    /// <param name="destinationPath">The path of destination image.</param>
    /// <param name="compressionLevel">The compression level.</param>
    public static void CompressImageAndSaveAs(string sourcePath, string destinationPath, ImageCompressionLevel compressionLevel)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourcePath, nameof(sourcePath));
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationPath, nameof(destinationPath));

        var format = FindFormatByFileExtension(destinationPath);
        using var image = Image.Load(sourcePath);
        var encoder = format.GetImageEncoder(compressionLevel);
        image.Save(destinationPath, encoder);
    }
    #endregion

    #region CompressImageBytesAndSaveAs
    /// <summary>
    /// Compresses the specified path.
    /// </summary>
    /// <param name="bytes">The bytes of the image.</param>
    /// <param name="path">The path.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static async Task CompressImageBytesAndSaveAsAsync(byte[] bytes, string path, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNullOrEmpty(bytes, nameof(bytes));
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        var format = FindFormatByFileExtension(path);
        using var image = Image.Load(bytes);
        var encoder = format.GetImageEncoder();
        DirectoryHelper.EnsureDirectoryExists(path);
        await image.SaveAsync(path, encoder, cancellationToken);
    }

    /// <summary>
    /// Compresses the specified path.
    /// </summary>
    /// <param name="bytes">The bytes of the image.</param>
    /// <param name="path">The path.</param>
    /// <param name="compressionLevel">The compression level.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static async Task CompressImageBytesAndSaveAsAsync(byte[] bytes, string path, ImageCompressionLevel compressionLevel, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNullOrEmpty(bytes, nameof(bytes));
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        var format = FindFormatByFileExtension(path);
        using var image = Image.Load(bytes);
        var encoder = format.GetImageEncoder(compressionLevel);
        DirectoryHelper.EnsureDirectoryExists(path);
        await image.SaveAsync(path, encoder, cancellationToken);
    }

    /// <summary>
    /// Compresses the specified path.
    /// </summary>
    /// <param name="bytes">The bytes of the image.</param>
    /// <param name="path">The path.</param>
    public static void CompressImageBytesAndSaveAs(byte[] bytes, string path)
    {
        Guard.ThrowIfNullOrEmpty(bytes, nameof(bytes));
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        var format = FindFormatByFileExtension(path);
        using var image = Image.Load(bytes);
        var encoder = format.GetImageEncoder();
        DirectoryHelper.EnsureDirectoryExists(path);
        image.Save(path, encoder);
    }

    /// <summary>
    /// Compresses the specified path.
    /// </summary>
    /// <param name="bytes">The bytes of the image.</param>
    /// <param name="path">The path.</param>
    /// <param name="compressionLevel">The compression level.</param>
    public static void CompressImageBytesAndSaveAs(byte[] bytes, string path, ImageCompressionLevel compressionLevel)
    {
        Guard.ThrowIfNullOrEmpty(bytes, nameof(bytes));
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        var format = FindFormatByFileExtension(path);
        using var image = Image.Load(bytes);
        var encoder = format.GetImageEncoder(compressionLevel);
        DirectoryHelper.EnsureDirectoryExists(path);
        image.Save(path, encoder);
    }
    #endregion

    #region CompressImageAndSave
    /// <summary>
    /// Compresses the specified path asynchronously.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static async Task CompressImageAndSaveAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        using var image = await Image.LoadAsync(path, cancellationToken);
        var encoder = image.Metadata.GetDecodedImageFormat()!.GetImageEncoder();
        DirectoryHelper.EnsureDirectoryExists(path);
        await image.SaveAsync(path, encoder, cancellationToken);
    }

    /// <summary>
    /// Compresses the specified path asynchronously.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="compressionLevel">The compression level.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static async Task CompressImageAndSaveAsync(string path, ImageCompressionLevel compressionLevel, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        using var image = await Image.LoadAsync(path, cancellationToken);
        var encoder = image.Metadata.GetDecodedImageFormat()!.GetImageEncoder(compressionLevel);
        DirectoryHelper.EnsureDirectoryExists(path);
        await image.SaveAsync(path, encoder, cancellationToken);
    }

    /// <summary>
    /// Compresses the specified path.
    /// </summary>
    /// <param name="path">The path.</param>
    public static void CompressImageAndSave(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        using var image = Image.Load(path);
        var encoder = image.Metadata.GetDecodedImageFormat()!.GetImageEncoder();
        DirectoryHelper.EnsureDirectoryExists(path);
        image.Save(path, encoder);
    }

    /// <summary>
    /// Compresses the specified path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="compressionLevel">The compression level.</param>
    public static void CompressImageAndSave(string path, ImageCompressionLevel compressionLevel)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        using var image = Image.Load(path);
        var encoder = image.Metadata.GetDecodedImageFormat()!.GetImageEncoder(compressionLevel);
        DirectoryHelper.EnsureDirectoryExists(path);
        image.Save(path, encoder);
    }
    #endregion

    #region CompressImageBytes
    /// <summary>
    /// Compresses the specified bytes.
    /// </summary>
    /// <param name="bytes">The bytes of the image.</param>
    /// <returns>The compressed bytes of the image.</returns>
    public static byte[] CompressImageBytes(byte[] bytes)
    {
        Guard.ThrowIfNullOrEmpty(bytes, nameof(bytes));

        using var image = Image.Load(bytes);
        var encoder = image.Metadata.GetDecodedImageFormat()!.GetImageEncoder();
        var outputStream = new MemoryStream();
        image.Save(outputStream, encoder);
        return outputStream.GetTrimmedBuffer();
    }

    /// <summary>
    /// Compresses the specified bytes.
    /// </summary>
    /// <param name="bytes">The bytes of the image.</param>
    /// <param name="compressionLevel">The compression level.</param>
    /// <returns>The compressed bytes of the image.</returns>
    public static byte[] CompressImageBytes(byte[] bytes, ImageCompressionLevel compressionLevel)
    {
        Guard.ThrowIfNullOrEmpty(bytes, nameof(bytes));

        using var image = Image.Load(bytes);
        var encoder = image.Metadata.GetDecodedImageFormat()!.GetImageEncoder(compressionLevel);
        var outputStream = new MemoryStream();
        image.Save(outputStream, encoder);
        return outputStream.GetTrimmedBuffer();
    }

    /// <summary>
    /// Compresses the specified bytes.
    /// </summary>
    /// <param name="bytes">The bytes of the image.</param>
    /// <param name="outputFormat">The format.</param>
    /// <returns>The compressed bytes of the image.</returns>
    public static byte[] CompressImageBytes(byte[] bytes, ImageFormat outputFormat)
    {
        Guard.ThrowIfNullOrEmpty(bytes, nameof(bytes));

        using var image = Image.Load(bytes);
        var encoder = outputFormat.GetImageEncoder();
        var outputStream = new MemoryStream();
        image.Save(outputStream, encoder);
        return outputStream.GetTrimmedBuffer();
    }

    /// <summary>
    /// Compresses the specified bytes.
    /// </summary>
    /// <param name="bytes">The bytes of the image.</param>
    /// <param name="outputFormat">The format.</param>
    /// <param name="compressionLevel">The compression level.</param>
    /// <returns>The compressed bytes of the image.</returns>
    public static byte[] CompressImageBytes(byte[] bytes, ImageFormat outputFormat, ImageCompressionLevel compressionLevel)
    {
        Guard.ThrowIfNullOrEmpty(bytes, nameof(bytes));

        using var image = Image.Load(bytes);
        var encoder = outputFormat.GetImageEncoder(compressionLevel);
        var outputStream = new MemoryStream();
        image.Save(outputStream, encoder);
        return outputStream.GetTrimmedBuffer();
    }
    #endregion

    #region ImageFormat & ImageCompressionLevel
    /// <summary>
    /// Finds the image format (<see cref="IImageFormat"/>) by file extension.
    /// </summary>
    /// <param name="fileExtension">The file extension.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IImageFormat FindFormatByFileExtension(string fileExtension)
    {
        fileExtension = Path.GetExtension(fileExtension);
        if (Configuration.Default.ImageFormatsManager.TryFindFormatByFileExtension(fileExtension, out var format) is false)
            throw new InvalidOperationException();
        return format;
    }

    /// <summary>
    /// Gets the image encoder based on image format.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="compressionLevel">The compression level.</param>
    /// <returns></returns>
    public static IImageEncoder GetImageEncoder(this IImageFormat format, ImageCompressionLevel? compressionLevel = null)
    {
        return format switch
        {
            PngFormat => new PngEncoder { CompressionLevel = (compressionLevel ?? ImageCompressionLevel.DefaultCompressionForPng).ToPngCompressionLevel() },
            JpegFormat => new JpegEncoder { Quality = (compressionLevel ?? ImageCompressionLevel.DefaultCompressionForJpeg).ToJpegQualityLevel() },
            WebpFormat => new WebpEncoder { Quality = (compressionLevel ?? ImageCompressionLevel.DefaultCompressionForWebp).ToWebpQualityLevel() },
            _ => throw new NotSupportedException($"Type of {format} is not supported.")
        };
    }

    /// <summary>
    /// Gets the image encoder based on image format.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="compressionLevel">The compression level.</param>
    /// <returns></returns>
    public static IImageEncoder GetImageEncoder(this ImageFormat format, ImageCompressionLevel? compressionLevel = null)
    {
        return format switch
        {
            ImageFormat.Png => new PngEncoder { CompressionLevel = (compressionLevel ?? ImageCompressionLevel.DefaultCompressionForPng).ToPngCompressionLevel() },
            ImageFormat.Jpeg => new JpegEncoder { Quality = (compressionLevel ?? ImageCompressionLevel.DefaultCompressionForJpeg).ToJpegQualityLevel() },
            ImageFormat.Webp => new WebpEncoder { Quality = (compressionLevel ?? ImageCompressionLevel.DefaultCompressionForWebp).ToWebpQualityLevel() },
            _ => throw new InvalidEnumArgumentException(nameof(format), Convert.ToInt32(format), format.GetType()),
        };
    }

    /// <summary>
    /// Converts to <see cref="PngCompressionLevel"/>.
    /// </summary>
    /// <param name="compressionLevel">The image compression level.</param>
    /// <returns></returns>
    public static PngCompressionLevel ToPngCompressionLevel(this ImageCompressionLevel compressionLevel)
    {
        var value = (int)compressionLevel / 10;
        if (value == 10)
            value = 9;
        return (PngCompressionLevel)value;
    }

    /// <summary>
    /// Converts to value that is suitable for JpegEncoder.Quality.
    /// </summary>
    /// <param name="compressionLevel">The image compression level.</param>
    /// <returns></returns>
    public static int ToJpegQualityLevel(this ImageCompressionLevel compressionLevel)
    {
        var value = 100 - (int)compressionLevel;
        if (value == 0)
            value = 1;
        return value;
    }

    /// <summary>
    /// Converts to value that is suitable for WebpEncoder.Quality.
    /// </summary>
    /// <param name="compressionLevel">The image compression level.</param>
    /// <returns></returns>
    public static int ToWebpQualityLevel(this ImageCompressionLevel compressionLevel)
    {
#if NET6_0_OR_GREATER
        //Webp compression is different in newer versions
        return (int)compressionLevel;
#else
        return 100 - (int)compressionLevel;
#endif
    }
    #endregion

    #region SkiaSharp
    //<PackageReference Include="SkiaSharp" Version="2.88.6" />
    //public static class SkiaSharp
    //{
    //    /// <summary>
    //    /// Compresses the JPEG.
    //    /// </summary>
    //    /// <param name="path">The path.</param>
    //    /// <param name="quality">The quality.</param>
    //    public static void CompressJpeg(string path, int quality = 80)
    //    {
    //        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));
    //        Guard.ThrowIfNotInRange(quality, 0, 100);

    //        using var bitmap = SKBitmap.Decode(path);
    //        using var compressedData = bitmap.Encode(SKEncodedImageFormat.Jpeg, quality);
    //        using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
    //        compressedData.SaveTo(fileStream);
    //        fileStream.Flush();
    //    }

    //    /// <summary>
    //    /// Compresses the PNG.
    //    /// </summary>
    //    /// <param name="path">The path.</param>
    //    /// <param name="quality">The quality.</param>
    //    public static void CompressPng(string path, int quality = 80)
    //    {
    //        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));
    //        Guard.ThrowIfNotInRange(quality, 0, 100);

    //        using var bitmap = SKBitmap.Decode(path);
    //        using var compressedData = bitmap.Encode(SKEncodedImageFormat.Png, quality);
    //        using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
    //        compressedData.SaveTo(fileStream);
    //        fileStream.Flush();
    //    }

    //    /// <summary>
    //    /// Compresses the webp.
    //    /// </summary>
    //    /// <param name="path">The path.</param>
    //    /// <param name="quality">The quality.</param>
    //    public static void CompressWebp(string path, int quality = 80)
    //    {
    //        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));
    //        Guard.ThrowIfNotInRange(quality, 0, 100);

    //        using var bitmap = SKBitmap.Decode(path);
    //        using var compressedData = bitmap.Encode(SKEncodedImageFormat.Webp, quality);
    //        using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
    //        compressedData.SaveTo(fileStream);
    //        fileStream.Flush();
    //    }

    //    /// <summary>
    //    /// Compresses the JPEG.
    //    /// </summary>
    //    /// <param name="bytes">The bytes.</param>
    //    /// <param name="quality">The quality.</param>
    //    /// <returns></returns>
    //    public static byte[] CompressJpeg(ReadOnlySpan<byte> bytes, int quality = 80)
    //    {
    //        if (bytes.Length == 0) throw new ArgumentException("Argument is empty.", nameof(bytes));
    //        Guard.ThrowIfNotInRange(quality, 0, 100);

    //        using var bitmap = SKBitmap.Decode(bytes);
    //        using var compressedData = bitmap.Encode(SKEncodedImageFormat.Jpeg, quality);
    //        return compressedData.ToArray(); //compressedData.Span;
    //    }

    //    /// <summary>
    //    /// Compresses the PNG.
    //    /// </summary>
    //    /// <param name="bytes">The bytes.</param>
    //    /// <param name="quality">The quality.</param>
    //    /// <returns></returns>
    //    public static byte[] CompressPng(ReadOnlySpan<byte> bytes, int quality = 80)
    //    {
    //        if (bytes.Length == 0) throw new ArgumentException("Argument is empty.", nameof(bytes));
    //        Guard.ThrowIfNotInRange(quality, 0, 100);

    //        using var bitmap = SKBitmap.Decode(bytes);
    //        using var compressedData = bitmap.Encode(SKEncodedImageFormat.Png, quality);
    //        return compressedData.ToArray();
    //    }

    //    /// <summary>
    //    /// Compresses the webp.
    //    /// </summary>
    //    /// <param name="bytes">The bytes.</param>
    //    /// <param name="quality">The quality.</param>
    //    /// <returns></returns>
    //    public static byte[] CompressWebp(ReadOnlySpan<byte> bytes, int quality = 80)
    //    {
    //        if (bytes.Length == 0) throw new ArgumentException("Argument is empty.", nameof(bytes));
    //        Guard.ThrowIfNotInRange(quality, 0, 100);

    //        using var bitmap = SKBitmap.Decode(bytes);
    //        using var compressedData = bitmap.Encode(SKEncodedImageFormat.Webp, quality);
    //        return compressedData.ToArray();
    //    }
    //}
    #endregion
}