namespace BenchmarkDotNetVisualizer;

/// <summary>
/// Directory Helper
/// </summary>
public static class DirectoryHelper
{
    #region File Write
    /// <summary>
    /// Writes the text to project directory or absolute path asynchronously.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="text">The text.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task WriteTextToProjectDirectoryOrAbsolutePathAsync(string path, string text, CancellationToken cancellationToken = default)
    {
        path = IsAbsolutePath(ref path) ? path : GetPathRelativeToProjectDirectory(path);

        var directory = Path.GetDirectoryName(path);
        if (string.IsNullOrWhiteSpace(directory) is false && Directory.Exists(directory) is false)
            Directory.CreateDirectory(directory!);

        return File.WriteAllTextAsync(path, text, cancellationToken);
    }

    /// <summary>
    /// Writes the bytes to project directory or absolute path asynchronously.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="bytes">The bytes.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task WriteBytesToProjectDirectoryOrAbsolutePathAsync(string path, byte[] bytes, CancellationToken cancellationToken = default)
    {
        path = IsAbsolutePath(ref path) ? path : GetPathRelativeToProjectDirectory(path);

        EnsureDirectoryExists(path);

        return File.WriteAllBytesAsync(path, bytes, cancellationToken);
    }

    /// <summary>
    /// Writes the text to project directory or absolute path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="text">The text.</param>
    public static void WriteTextToProjectDirectoryOrAbsolutePath(string path, string text)
    {
        path = IsAbsolutePath(ref path) ? path : GetPathRelativeToProjectDirectory(path);

        EnsureDirectoryExists(path);

        File.WriteAllText(path, text);
    }

    /// <summary>
    /// Writes the bytes to project directory or absolute path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="bytes">The bytes.</param>
    public static void WriteBytesToProjectDirectoryOrAbsolutePath(string path, byte[] bytes)
    {
        path = IsAbsolutePath(ref path) ? path : GetPathRelativeToProjectDirectory(path);

        EnsureDirectoryExists(path);

        File.WriteAllBytes(path, bytes);
    }
    #endregion

    /// <summary>
    /// Moves (cuts) the benchmark artifacts to the project directory.
    /// </summary>
    /// <param name="directory">The directory.</param>
    /// <param name="includeLogs">if set to <c>true</c> includes .log files.</param>
    public static void MoveBenchmarkArtifactsToProjectDirectory(string directory = "", bool includeLogs = false)
    {
        string destinationDir;
        string artifactsDir;

        if (includeLogs)
        {
            destinationDir = Path.Combine(GetPathRelativeToProjectDirectory(directory), "BenchmarkDotNet.Artifacts");
            artifactsDir = GetBenchmarkArtifactsDirectory();
        }
        else
        {
            destinationDir = Path.Combine(GetPathRelativeToProjectDirectory(directory), "BenchmarkDotNet.Artifacts", "results");
            artifactsDir = GetBenchmarkArtifactResultsDirectory();
        }

        MoveDirectory(artifactsDir, destinationDir);
    }

    /// <summary>
    /// Gets the path relative to project directory.
    /// </summary>
    /// <param name="directory">The directory.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">The path '{directory}' is not relative.'</exception>
    public static string GetPathRelativeToProjectDirectory(string directory)
    {
        ArgumentNullException.ThrowIfNull(directory, nameof(directory));

        if (IsAbsolutePath(ref directory))
            throw new InvalidOperationException($"The path '{directory}' is not relative.'");

        var projectDir = GetProjectDirectory();
        return Path.GetFullPath(directory, projectDir);
    }

    /// <summary>
    /// Deletes the benchmark artifacts directory.
    /// </summary>
    public static void DeleteBenchmarkArtifactsDirectory()
    {
        var artifactsDir = GetBenchmarkArtifactsDirectory();
        if (Directory.Exists(artifactsDir))
            Directory.Delete(artifactsDir, recursive: true);
    }

    /// <summary>
    /// Gets the project benchmark artifacts directory.
    /// </summary>
    /// <param name="directory">The directory.</param>
    /// <returns></returns>
    public static string GetProjectBenchmarkArtifactsDirectory(string directory = "")
    {
        var projectDirectory = GetPathRelativeToProjectDirectory(directory);
        return Path.Combine(projectDirectory, "BenchmarkDotNet.Artifacts");
    }

    /// <summary>
    /// Gets the project benchmark artifacts results directory.
    /// </summary>
    /// <param name="directory">The directory.</param>
    /// <returns></returns>
    public static string GetProjectBenchmarkArtifactResultsDirectory(string directory = "")
    {
        var projectDirectory = GetPathRelativeToProjectDirectory(directory);
        return Path.Combine(projectDirectory, "BenchmarkDotNet.Artifacts", "results");
    }

    /// <summary>
    /// Gets the benchmark artifacts directory.
    /// </summary>
    /// <returns></returns>
    public static string GetBenchmarkArtifactsDirectory()
    {
        var currentDirectory = AppContext.BaseDirectory;
        return Path.Combine(currentDirectory, "BenchmarkDotNet.Artifacts");
    }

    /// <summary>
    /// Gets the benchmark artifacts results directory.
    /// </summary>
    /// <returns></returns>
    public static string GetBenchmarkArtifactResultsDirectory()
    {
        var currentDirectory = AppContext.BaseDirectory;
        return Path.Combine(currentDirectory, "BenchmarkDotNet.Artifacts", "results");
    }

    /// <summary>
    /// Gets the project directory.
    /// </summary>
    /// <returns></returns>
    public static string GetProjectDirectory()
    {
        var currentDirectory = AppContext.BaseDirectory;
        var path = Path.Combine(currentDirectory, "..", "..", "..");
        return Path.GetFullPath(path);
    }

    /// <summary>
    /// Determines whether the specified path is absolute path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>
    ///   <c>true</c> if  the specified path is absolute path; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsAbsolutePath(ref string path)
    {
        //To bypass such this issue (only in windows): Path.IsPathRooted(@"\a\b") is true! and FullPath (absolute path) is "C:\a\b"
        path = path.TrimStart('\\');

        return Path.IsPathRooted(path);
    }

    /// <summary>
    /// Ensures the directory exists.
    /// </summary>
    /// <param name="path">The path.</param>
    public static void EnsureDirectoryExists(string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (string.IsNullOrWhiteSpace(directory) is false && Directory.Exists(directory) is false)
            Directory.CreateDirectory(directory!);
    }

    /// <summary>
    /// Moves the directory from source path to destination path.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="target">The target.</param>
    /// <param name="overwriteExistedFiles">if set to <c>true</c> overwrites existed files, otherwise ignores it.</param>
    public static void MoveDirectory(string source, string target, bool overwriteExistedFiles = true)
    {
        var sourcePath = source.TrimEnd('\\', ' ');
        var targetPath = target.TrimEnd('\\', ' ');
        var files = Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories)
                             .GroupBy(Path.GetDirectoryName);
        foreach (var folder in files)
        {
            var targetFolder = folder.Key!.Replace(sourcePath, targetPath);
            Directory.CreateDirectory(targetFolder);
            foreach (var file in folder)
            {
                var targetFile = Path.Combine(targetFolder, Path.GetFileName(file));

                if (File.Exists(targetFile))
                {
                    if (overwriteExistedFiles)
                    {
                        File.Delete(targetFile);
                        File.Move(file, targetFile);
                    }
                    //else, DO Nothing and just ignore the already existed file
                }
                else
                {
                    File.Move(file, targetFile);
                }
            }
        }
        Directory.Delete(source, true);
    }
}