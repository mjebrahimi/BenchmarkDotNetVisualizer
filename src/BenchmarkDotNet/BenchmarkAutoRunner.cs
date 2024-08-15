using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNetVisualizer.Utilities;
using System.Diagnostics;
using System.Reflection;

namespace BenchmarkDotNetVisualizer;

/// <summary>
/// Benchmark Auto Runner
/// </summary>
public static class BenchmarkAutoRunner
{
    #region BenchmarkRunner
    /// <summary>
    /// Runs the benchmark of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="args">The arguments.</param>
    /// <returns></returns>
    public static Summary Run<T>(string[]? args = null)
    {
        PrintMessages();
        DownloadBrowserInBackgroundIfEnabled();
        return BenchmarkRunner.Run<T>(GetConfig(), args);
    }

    /// <summary>
    /// Runs the benchmark of the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="args">The arguments.</param>
    /// <returns></returns>
    public static Summary Run(Type type, string[]? args = null)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));

        PrintMessages();
        DownloadBrowserInBackgroundIfEnabled();
        return BenchmarkRunner.Run(type, GetConfig(), args);
    }

    /// <summary>
    /// Runs the benchmarks of the specified <paramref name="types"/>.
    /// </summary>
    /// <param name="types">The types.</param>
    /// <param name="args">The arguments.</param>
    /// <returns></returns>
    public static Summary[] Run(Type[] types, string[]? args = null)
    {
        Guard.ThrowIfNullOrEmpty(types, nameof(types));

        PrintMessages();
        DownloadBrowserInBackgroundIfEnabled();
        return BenchmarkRunner.Run(types, GetConfig(), args);
    }

    /// <summary>
    /// Runs all the benchmarks of the specified <paramref name="assembly"/>.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <param name="args">The arguments.</param>
    /// <returns></returns>
    public static Summary[] Run(Assembly assembly, string[]? args = null)
    {
        ArgumentNullException.ThrowIfNull(assembly, nameof(assembly));

        PrintMessages();
        DownloadBrowserInBackgroundIfEnabled();
        return BenchmarkRunner.Run(assembly, GetConfig(), args);
    }
    #endregion

    #region BenchmarkSwitcher
    /// <summary>
    /// Runs switcher of the benchmarks of the specified <paramref name="types"/>.
    /// </summary>
    /// <param name="types">The types.</param>
    /// <param name="args">The arguments.</param>
    /// <returns></returns>
    public static Summary[] SwitcherRun(Type[] types, string[]? args = null)
    {
        Guard.ThrowIfNullOrEmpty(types, nameof(types));

        PrintMessages();
        DownloadBrowserInBackgroundIfEnabled();
        return BenchmarkSwitcher.FromTypes(types).Run(args, GetConfig()).ToArray();
    }

    /// <summary>
    /// Runs switcher of the benchmarks of the specified <paramref name="assembly"/>.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <param name="args">The arguments.</param>
    /// <returns></returns>
    public static Summary[] SwitcherRun(Assembly assembly, string[]? args = null)
    {
        ArgumentNullException.ThrowIfNull(assembly, nameof(assembly));

        PrintMessages();
        DownloadBrowserInBackgroundIfEnabled();
        return BenchmarkSwitcher.FromAssembly(assembly).Run(args, GetConfig()).ToArray();
    }
    #endregion

    /// <summary>
    /// Gets the configuration according to is project running in <see langword="DEBUG"/> mode (returns <see cref="DebugInProcessConfigDry"/>) or <see langword="NOT"/> (returns <see langword="null"/>).
    /// </summary>
    /// <returns></returns>
    public static IConfig? GetConfig()
    {
        return IsRunningInDebugMode() ? new DebugInProcessConfigDry() : null;
    }

    /// <summary>
    /// Prints the messages according to is project running in  <see langword="DEBUG"/> mode or <see cref="Debugger.IsAttached"/>
    /// </summary>
    private static void PrintMessages()
    {
        if (IsRunningInDebugMode())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("You are in DEBUG mode. To achieve accurate results, set project configuration to RELEASE mode.");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Please wait 3 seconds to enter DEBUG mode");

            Thread.Sleep(3000);
        }
        else if (Debugger.IsAttached)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Debugger is Attached. To achieve accurate results, run project without Debugger.");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Or if you want to Debug project, set project configuration to DEBUG mode.");

            Console.ForegroundColor = ConsoleColor.White;
            Environment.Exit(0);
        }
    }

    /// <summary>
    /// Determines whether project is running in <see langword="DEBUG"/> mode.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if project is running in <see langword="DEBUG"/> mode; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsRunningInDebugMode()
    {
        //NOTE: #if DEBUG or [Conditional("DEBUG")] don't work for libraries because these are applied in compile-time not run-time
        var customAttributes = Assembly.GetEntryAssembly()!.GetCustomAttributes(typeof(DebuggableAttribute), false);
        if (customAttributes?.Length == 1)
        {
            var attribute = (DebuggableAttribute)customAttributes[0];
            return attribute.IsJITOptimizerDisabled && attribute.IsJITTrackingEnabled;
        }
        return false;
    }

    /// <summary>
    /// Gets or sets a value indicating whether download the browser in background.
    /// </summary>
    public static bool DownloadBrowserInBackground { get; set; } = true;

    /// <summary>
    /// Downloads the browser in background if <see cref="DownloadBrowserInBackground"/> is enabled.
    /// </summary>
    private static void DownloadBrowserInBackgroundIfEnabled()
    {
        if (DownloadBrowserInBackground is false)
            return;

        Task.Run(async () =>
        {
            if (HtmlHelper.DefaultBrowser is not null)
                return;

            try
            {
                Console.WriteLine("Download Browser Started in Background.");
                await HtmlHelper.EnsureBrowserDownloadedAsync(silent: true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        });
    }
}