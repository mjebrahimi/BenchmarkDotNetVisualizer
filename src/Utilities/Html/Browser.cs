using PuppeteerSharp;

namespace BenchmarkDotNetVisualizer.Utilities;

/// <summary>
/// Represents a browser configuration.
/// </summary>
public class Browser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Browser"/> class.
    /// </summary>
    /// <param name="browserType">Type of the browser.</param>
    /// <param name="executablePath">The executable path.</param>
    public Browser(SupportedBrowser browserType, string executablePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(executablePath, nameof(executablePath));

        BrowserType = browserType;
        ExecutablePath = executablePath;
    }

    /// <summary>
    /// Gets or sets the type of the browser.
    /// </summary>
    /// <value>
    /// The type of the browser.
    /// </value>
    public SupportedBrowser BrowserType { get; set; }

    /// <summary>
    /// Gets or sets the executable path.
    /// </summary>
    /// <value>
    /// The executable path.
    /// </value>
    public string ExecutablePath { get; set; }
}
