namespace BenchmarkDotNetVisualizer.Utilities;

#pragma warning disable CA1069, RCS1234 // Enums values should not be duplicated
/// <summary>
/// Image Compression Level
/// </summary>
public enum ImageCompressionLevel
{
    /// <summary>
    /// No compression. <c>Equivalent to <see cref="Level0"/></c>
    /// </summary>
    NoCompression = 0,
    /// <summary>
    /// Best speed compression level. <c>(Equivalent to <see cref="Level10"/>)</c>
    /// </summary>
    BestSpeed = 10,
    /// <summary>
    /// The default compression level. <c>(Equivalent to <see cref="Level40"/>)</c>
    /// </summary>
    DefaultCompressionForJpeg = 40,
    /// <summary>
    /// The default compression level. <c>(Equivalent to <see cref="Level100"/>)</c>
    /// </summary>
    DefaultCompressionForPng = 100,
#if NET6_0_OR_GREATER
    /// <summary>
    /// The default compression level. <c>(Equivalent to <see langword="Level100"/>)</c>
    /// </summary>
    DefaultCompressionForWebp = 100,
#else
    /// <summary>
    /// The default compression level. <c>(Equivalent to <see langword="Level40"/>)</c>
    /// </summary>
    DefaultCompressionForWebp = 40,
#endif
    /// <summary>
    /// Best compression level. <c>(Equivalent to <see cref="Level100"/>)</c>
    /// </summary>
    BestCompression = 100,

    /// <summary>
    /// Level 0.
    /// </summary>
    Level0 = 0,
    /// <summary>
    /// Level 10.
    /// </summary>
    Level10 = 10,
    /// <summary>
    /// Level 20.
    /// </summary>
    Level20 = 20,
    /// <summary>
    /// Level 30.
    /// </summary>
    Level30 = 30,
    /// <summary>
    /// Level 40.
    /// </summary>
    Level40 = 40,
    /// <summary>
    /// Level 50.
    /// </summary>
    Level50 = 50,
    /// <summary>
    /// Level 60.
    /// </summary>
    Level60 = 60,
    /// <summary>
    /// Level 70.
    /// </summary>
    Level70 = 70,
    /// <summary>
    /// Level 80.
    /// </summary>
    Level80 = 80,
    /// <summary>
    /// Level 90.
    /// </summary>
    Level90 = 90,
    /// <summary>
    /// Level 100.
    /// </summary>
    Level100 = 100
}
#pragma warning restore CA1069, RCS1234 // Enums values should not be duplicated