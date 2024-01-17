namespace BenchmarkDotNetVisualizer.Utilities;

/// <summary>
/// Stream Extensions
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    /// Gets the trimmed buffer of the specified <see cref="MemoryStream"/>.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <remarks>
    /// Use it only if you are the owner/creator of the <see cref="MemoryStream"/> instance, NOT when it came from somewhere else.
    /// Otherwise it can cause problems
    /// </remarks>
    /// <returns></returns>
    public static byte[] GetTrimmedBuffer(this MemoryStream stream)
    {
        var bytes = stream.GetBuffer();
        Array.Resize(ref bytes, (int)stream.Length);
        return bytes;
    }
}