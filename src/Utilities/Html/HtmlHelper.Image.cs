// Ignore Spelling: Pdf

using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.Dynamic;

namespace BenchmarkDotNetVisualizer.Utilities;

/// <summary>
/// Html Helper for rendering Image and Pdf using Chrome headless browser
/// </summary>
public static partial class HtmlHelper
{
    /// <summary>
    /// Renders to image and save asynchronously.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="path">The path to save.</param>
    /// <param name="dividerMode">The divider mode when generating html.</param>
    /// <param name="elementQuery">The element query to screen-shot.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task RenderToImageAsync(this IEnumerable<ExpandoObject?> source, string path,
        RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow, string elementQuery = "body", CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNullOrEmpty(source, nameof(source));
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));
        ArgumentException.ThrowIfNullOrWhiteSpace(elementQuery, nameof(elementQuery));

        var table = ToHtmlTable(source, dividerMode);
        var html = WrapInHtmlDocument(table, string.Empty, HtmlDocumentWrapMode.Simple);
        return RenderHtmlToImageAsync(html, path, elementQuery, cancellationToken);
    }

    /// <summary>
    /// Renders to image data asynchronously.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="dividerMode">The divider mode when generating html.</param>
    /// <param name="elementQuery">The element query to screen-shot.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task<byte[]> RenderToImageDataAsync(this IEnumerable<ExpandoObject?> source,
        RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow, string elementQuery = "body", CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNullOrEmpty(source, nameof(source));
        ArgumentException.ThrowIfNullOrWhiteSpace(elementQuery, nameof(elementQuery));

        var table = ToHtmlTable(source, dividerMode);
        var html = WrapInHtmlDocument(table, string.Empty, HtmlDocumentWrapMode.Simple);
        return RenderHtmlToImageDataAsync(html, elementQuery, cancellationToken);
    }

    /// <summary>
    /// Renders the HTML to image and save asynchronously.
    /// </summary>
    /// <param name="html">The HTML to render.</param>
    /// <param name="path">The path to save.</param>
    /// <param name="elementQuery">The element query to screen-shot.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static async Task RenderHtmlToImageAsync(string html, string path, string elementQuery = "body", CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(html, nameof(html));
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));
        ArgumentException.ThrowIfNullOrWhiteSpace(elementQuery, nameof(elementQuery));

        var imageBytes = await RenderHtmlToImageDataAsync(html, elementQuery, cancellationToken);
        //await File.WriteAllBytesAsync(path, imageBytes, cancellationToken); //Saving the original image
        await ImageHelper.CompressImageBytesAndSaveAsAsync(imageBytes, path, cancellationToken);
    }

    /// <summary>
    /// Renders the HTML to image data asynchronously.
    /// </summary>
    /// <param name="html">The HTML to render.</param>
    /// <param name="elementQuery">The element query to screen-shot.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static async Task<byte[]> RenderHtmlToImageDataAsync(string html, string elementQuery = "body", CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(html, nameof(html));
        ArgumentException.ThrowIfNullOrWhiteSpace(elementQuery, nameof(elementQuery));

        IBrowser browser = null!;

        //NOTE: It doesn't support cancellationToken built-in, this is just a workaround for support.
        cancellationToken.Register(() =>
        {
#pragma warning disable S2486 // Generic exceptions should not be ignored
#pragma warning disable AsyncFixer02 // Long-running or blocking operations inside an async method
            try { browser?.CloseAsync().Wait(); } catch { }
            try { browser?.Dispose(); } catch { }
#pragma warning restore AsyncFixer02 // Long-running or blocking operations inside an async method
#pragma warning restore S2486 // Generic exceptions should not be ignored
        });

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var browserFetcher = new BrowserFetcher();

            cancellationToken.ThrowIfCancellationRequested();

            // Download chrome (headless) browser (first time takes a while).
            await browserFetcher.DownloadAsync();

            cancellationToken.ThrowIfCancellationRequested();

            // Launch the browser and set the given html.
            await using var _ = browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });

            cancellationToken.ThrowIfCancellationRequested();

            await using var page = await browser.NewPageAsync();

            cancellationToken.ThrowIfCancellationRequested();

            await page.SetContentAsync(html);

            cancellationToken.ThrowIfCancellationRequested();

            // Wait for page to be idle or the selector to load.
            //await page.WaitForNetworkIdleAsync(new WaitForNetworkIdleOptions { Timeout = 2000 });
            await page.WaitForSelectorAsync(elementQuery, new WaitForSelectorOptions { Timeout = 2000 });

            cancellationToken.ThrowIfCancellationRequested();

            // Select the element and take a screen-shot
            var elementHandle = await page.QuerySelectorAsync(elementQuery);

            cancellationToken.ThrowIfCancellationRequested();

            var bounding = await elementHandle.BoundingBoxAsync(); // await elementHandle.BoxModelAsync();
            var options = new ScreenshotOptions
            {
                Clip = new Clip() { X = 0, Y = 0, Height = bounding.Height, Width = bounding.Width },
                Type = ScreenshotType.Png, // Poor quality with Jpeg/WebP.
                // Quality = Not supported with PNG/WebP images! (Only Jpeg)
            };

            cancellationToken.ThrowIfCancellationRequested();

            return await elementHandle.ScreenshotDataAsync(options);
        }
        finally
        {
            await browser.CloseAsync();
        }
    }

    /// <summary>
    /// Renders the HTML to PDF and save asynchronously.
    /// </summary>
    /// <param name="html">The HTML to print.</param>
    /// <param name="path">The path to save.</param>
    /// <param name="options">The PDF options.</param>
    /// <param name="mediaType">Type of the media.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <remarks>
    /// Options Example
    /// new PuppeteerSharp.PdfOptions
    /// {
    ///     PrintBackground = true, //colorful (true) or white-black (false)
    ///     Format = PaperFormat.A4, //use paper format or set width and height
    ///     Landscape = false,
    ///     PageRanges = "1-5, 8, 11-13",
    ///     DisplayHeaderFooter = false,
    ///     FooterTemplate = "[footer template]",
    ///     HeaderTemplate = "[header template]",
    ///     Tagged = false,
    ///     Scale = 1,
    ///     MarginOptions = new MarginOptions { Bottom = "10", Left = "10", Right = "10", Top = "10" },
    ///     OmitBackground = false,
    ///     PreferCSSPageSize = false,
    ///     Width = 1000,
    ///     Height = 2000
    /// }
    /// </remarks>
    /// <returns></returns>
    public static async Task RenderHtmlToPdfAsync(string html, string path, PdfOptions? options = null, MediaType? mediaType = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(html, nameof(html));
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        IBrowser browser = null!;

        //NOTE: It doesn't support cancellationToken built-in, this is just a workaround for support.
        cancellationToken.Register(() =>
        {
#pragma warning disable S2486 // Generic exceptions should not be ignored
#pragma warning disable AsyncFixer02 // Long-running or blocking operations inside an async method
            try { browser?.CloseAsync().Wait(); } catch { }
            try { browser?.Dispose(); } catch { }
#pragma warning restore AsyncFixer02 // Long-running or blocking operations inside an async method
#pragma warning restore S2486 // Generic exceptions should not be ignored
        });

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var browserFetcher = new BrowserFetcher();

            cancellationToken.ThrowIfCancellationRequested();

            // Download chrome (headless) browser (first time takes a while).
            await browserFetcher.DownloadAsync();

            cancellationToken.ThrowIfCancellationRequested();

            // Launch the browser and set the given html.
            await using var _ = browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });

            cancellationToken.ThrowIfCancellationRequested();

            await using var page = await browser.NewPageAsync();

            cancellationToken.ThrowIfCancellationRequested();

            await page.SetContentAsync(html);

            cancellationToken.ThrowIfCancellationRequested();

            // Wait for page to be idle or the selector to load.
            await page.WaitForNetworkIdleAsync(new WaitForNetworkIdleOptions { Timeout = 2000 });

            cancellationToken.ThrowIfCancellationRequested();

            if (mediaType.HasValue)
                await page.EmulateMediaTypeAsync(mediaType.Value);

            cancellationToken.ThrowIfCancellationRequested();

            if (options is null)
                await page.PdfAsync(path);
            else
                await page.PdfAsync(path, options);
        }
        finally
        {
            await browser.CloseAsync();
        }
    }
}