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
        //NOTE: #if Directives does not work because applies in compile-time not run-time
        //#if DEBUG
        //return true;
        //#else
        //return false;
        //#endif
        RunIfDebug();
        return debugging;
    }

    [Conditional("DEBUG")]
    private static void RunIfDebug()
    {
        debugging = true;
    }

    private static bool debugging;
}