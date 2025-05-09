﻿using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.Dynamic;

namespace BenchmarkDotNetVisualizer.Utilities;

/// <summary>
/// Html Helper for rendering Image and Pdf using Chrome headless browser
/// </summary>
public static partial class HtmlHelper
{
    private static readonly BrowserFetcher _browserFetcher = new();
    private static readonly SemaphoreSlim _browserDownloadLock = new(1, 1);
    private static readonly SemaphoreSlim _consolePrintLock = new(1, 1);

    /// <summary>
    /// Gets or sets the default browser
    /// </summary>
    public static Browser? DefaultBrowser { get; set; }

    static HtmlHelper()
    {
        var installedBrowsers = _browserFetcher.GetInstalledBrowsers().ToList();
        var defaultBrowser = installedBrowsers.Find(p => p.Browser == _browserFetcher.Browser && p.Platform == _browserFetcher.Platform);
        if (defaultBrowser is not null)
        {
            var executablePath = _browserFetcher.GetExecutablePath(defaultBrowser.BuildId);
            DefaultBrowser = new Browser(defaultBrowser.Browser, executablePath);
        }
    }

    /// <summary>
    /// Renders to image and save asynchronously.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="path">The path to save.</param>
    /// <param name="theme">The theme option</param>
    /// <param name="dividerMode">The divider mode when generating html.</param>
    /// <param name="elementQuery">The element query to screen-shot.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task RenderToImageAsync(this IEnumerable<ExpandoObject?> source, string path, Theme theme,
        RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow, string elementQuery = "body", CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNullOrEmpty(source, nameof(source));
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));
        ArgumentException.ThrowIfNullOrWhiteSpace(elementQuery, nameof(elementQuery));

        var table = ToHtmlTable(source, dividerMode);
        var html = WrapInHtmlDocument(table, string.Empty, theme, HtmlDocumentWrapMode.Simple);
        return RenderHtmlToImageAsync(html, path, elementQuery, cancellationToken);
    }

    /// <summary>
    /// Renders to image data asynchronously.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="theme">The theme option</param>
    /// <param name="dividerMode">The divider mode when generating html.</param>
    /// <param name="elementQuery">The element query to screen-shot.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task<byte[]> RenderToImageDataAsync(this IEnumerable<ExpandoObject?> source, Theme theme,
        RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow, string elementQuery = "body", CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNullOrEmpty(source, nameof(source));
        ArgumentException.ThrowIfNullOrWhiteSpace(elementQuery, nameof(elementQuery));

        var table = ToHtmlTable(source, dividerMode);
        var html = WrapInHtmlDocument(table, string.Empty, theme, HtmlDocumentWrapMode.Simple);
        html += "<style> .theme-toggle { display: none; } </style>"; //Hide theme-toggle button
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

        html += "<style> .theme-toggle { display: none; } </style>"; //Hide theme-toggle button
        var imageBytes = await RenderHtmlToImageDataAsync(html, elementQuery, cancellationToken);
        //await File.WriteAllBytesAsync(path, imageBytes, cancellationToken); //Saving the original image
        await ImageHelper.CompressImageBytesAndSaveAsAsync(imageBytes, path, cancellationToken);
    }

    /// <summary>
    /// Ensures that browser is downloaded, otherwise downloads browser with default configuration asynchronously.
    /// </summary>
    /// <param name="silent">Performs silently or prints logs to console output (Defaults to <see langword="false"/>)</param>
    public static async Task EnsureBrowserDownloadedAsync(bool silent = false)
    {
        var tasks = new List<Task>
        {
            Task.Run(async () =>
            {
                try
                {
                    await _browserDownloadLock.WaitAsync();

                    if (DefaultBrowser is not null)
                        return;

                    var installedBrowser = await _browserFetcher.DownloadAsync();
                    var executablePath = _browserFetcher.GetExecutablePath(installedBrowser.BuildId);
                    DefaultBrowser = new Browser(installedBrowser.Browser, executablePath);
                }
                finally
                {
                    _browserDownloadLock.Release();
                }
            })
        };

        await Task.Delay(1000);

        if (silent is false //print progress to console
            && await _consolePrintLock.IsLockAlreadyAcquiredAsync() is false //is not already printing to console
            && await _browserDownloadLock.IsLockAlreadyAcquiredAsync() //browser is downloading
            )
        {
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    await _consolePrintLock.WaitAsync();

                    var index = 0;
                    while (await _browserDownloadLock.IsLockAlreadyAcquiredAsync()) //browser is downloading
                    {
                        Console.Write($"Browser is downloading, please wait{new string('.', index + 1),-5}");
                        Console.SetCursorPosition(0, Console.CursorTop);
                        index = (index + 1) % 5;
                        await Task.Delay(500);
                    }
                    Console.WriteLine();
                    Console.WriteLine("Browser download is finished.");
                }
                finally
                {
                    _consolePrintLock.Release();
                }
            }));
        }

        await Task.WhenAll(tasks);
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

        IBrowser? browser = null;
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Downloads the browser (takes a while the first time)
            await EnsureBrowserDownloadedAsync();

            cancellationToken.ThrowIfCancellationRequested();

            // Launch the browser and set the given html.
            await using (browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Browser = DefaultBrowser!.BrowserType,
                ExecutablePath = DefaultBrowser!.ExecutablePath,
                //PuppeteerSharp.ProcessException : Failed to launch browser! [3163:3163:0420/000113.340765:FATAL:zygote_host_impl_linux.cc(128)] No usable sandbox!
                //If you are running on Ubuntu 23.10+ or another Linux distro that has disabled unprivileged user namespaces with AppArmor,
                //see https://chromium.googlesource.com/chromium/src/+/main/docs/security/apparmor-userns-restrictions.md
                //Otherwise see https://chromium.googlesource.com/chromium/src/+/main/docs/linux/suid_sandbox_development.md for more information on developing with the (older) SUID sandbox.
                //If you want to live dangerously and need an immediate workaround, you can try using --no-sandbox.
                //Solution: https://github.com/hardkoded/puppeteer-sharp/issues/2593
                Args = ["--no-sandbox"]
            }))
            {
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
                var options = new ElementScreenshotOptions
                {
                    Clip = new Clip() { X = 0, Y = 0, Height = bounding.Height, Width = bounding.Width },
                    Type = ScreenshotType.Png, // Poor quality with Jpeg/WebP.
                                               // Quality = Not supported with PNG/WebP images! (Only Jpeg)
                };

                cancellationToken.ThrowIfCancellationRequested();

                return await elementHandle.ScreenshotDataAsync(options);
            }
        }
        finally
        {
            if (browser is not null)
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

        IBrowser? browser = null;
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Downloads the browser (takes a while the first time)
            await EnsureBrowserDownloadedAsync();

            cancellationToken.ThrowIfCancellationRequested();

            // Launch the browser and set the given html.
            await using var _ = browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Browser = DefaultBrowser!.BrowserType,
                ExecutablePath = DefaultBrowser!.ExecutablePath,
                //PuppeteerSharp.ProcessException : Failed to launch browser! [3163:3163:0420/000113.340765:FATAL:zygote_host_impl_linux.cc(128)] No usable sandbox!
                //If you are running on Ubuntu 23.10+ or another Linux distro that has disabled unprivileged user namespaces with AppArmor,
                //see https://chromium.googlesource.com/chromium/src/+/main/docs/security/apparmor-userns-restrictions.md
                //Otherwise see https://chromium.googlesource.com/chromium/src/+/main/docs/linux/suid_sandbox_development.md for more information on developing with the (older) SUID sandbox.
                //If you want to live dangerously and need an immediate workaround, you can try using --no-sandbox.
                //Solution: https://github.com/hardkoded/puppeteer-sharp/issues/2593
                Args = ["--no-sandbox"]
            });

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
            if (browser is not null)
                await browser.CloseAsync();
        }
    }
}

internal static class SemaphoreSlimExtensions
{
    internal static async Task<bool> IsLockAlreadyAcquiredAsync(this SemaphoreSlim semaphoreSlim)
    {
        bool isAcquired = false;
        try
        {
            isAcquired = await semaphoreSlim.WaitAsync(0);
            return isAcquired is false;
        }
        finally
        {
            if (isAcquired)
                semaphoreSlim.Release();
        }
    }
}