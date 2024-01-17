using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.Emit;

namespace BenchmarkDotNetVisualizer;

/// <summary>
/// Similar to DebugInProcessConfig but uses Job.Dry instead of Job.Default.
/// </summary>
public class DebugInProcessConfigDry : DebugConfig
{
    /// <summary>
    /// Gets the Job.Dry instead of Job.Default.
    /// </summary>
    /// <returns></returns>
    public override IEnumerable<Job> GetJobs() => [
            Job.Dry // Job.Dry instead of Job.Default
                .WithToolchain(InProcessEmitToolchain.Instance) //InProcessNoEmitToolchain (A toolchain to run the benchmarks in-process (no emit))
        ];
}