using BenchmarkDotNetVisualizer.Utilities;
using System.Runtime.CompilerServices;
using VerifyTests.DiffPlex;

namespace BenchmarkDotNetVisualizer.Tests;

public abstract class TestBase
{
    protected static readonly string _benchmarkArtifactsPath = DirectoryHelper.GetPathRelativeToProjectDirectory("BenchmarkDotNet.Artifacts");
    private static readonly string _snapshotsPath = DirectoryHelper.GetPathRelativeToProjectDirectory("_snapshots");
    private static readonly string _reportsPath = DirectoryHelper.GetPathRelativeToProjectDirectory("_reports");

    static TestBase()
    {
        VerifyDiffPlex.Initialize(OutputType.Compact);
        DerivePathInfo((_, _, type, method) => new PathInfo(_snapshotsPath, type.Name, method.Name));
        HtmlHelper.EnsureBrowserDownloadedAsync(true).GetAwaiter().GetResult();
    }

    public async Task<(string htmlPath, string imgPath)> JoinReportsAndSaveAsHtmlAndImageAsync(BenchmarkInfo[] benchmarkInfo, JoinReportHtmlOptions options,
        [CallerFilePath] string callerFilePath = null!, [CallerMemberName] string callerMethod = null!)
    {
        var (htmlPath, imgPath) = GetHtmlAndImagePath(callerFilePath, callerMethod);

        await benchmarkInfo.JoinReportsAndSaveAsHtmlAndImageAsync(htmlPath, imgPath, options);

        return (htmlPath, imgPath);
    }

    public async Task<(string htmlPath, string imgPath)> JoinReportsAndSaveAsHtmlAndImageAsync(BenchmarkInfo benchmarkInfo, JoinReportHtmlOptions options,
        [CallerFilePath] string callerFilePath = null!, [CallerMemberName] string callerMethod = null!)
    {
        var (htmlPath, imgPath) = GetHtmlAndImagePath(callerFilePath, callerMethod);

        await benchmarkInfo.JoinReportsAndSaveAsHtmlAndImageAsync(htmlPath, imgPath, options);

        return (htmlPath, imgPath);
    }

    public async Task<(string htmlPath, string imgPath)> SaveAsHtmlAndImageAsync(BenchmarkInfo benchmarkInfo, ReportHtmlOptions options,
        [CallerFilePath] string callerFilePath = null!, [CallerMemberName] string callerMethod = null!)
    {
        var (htmlPath, imgPath) = GetHtmlAndImagePath(callerFilePath, callerMethod);

        await benchmarkInfo.SaveAsHtmlAndImageAsync(htmlPath, imgPath, options);

        return (htmlPath, imgPath);
    }

    #region Verify
    public async Task VerifyHtmlAndImage(string htmlPath, string imgPath,
        [CallerFilePath] string callerFilePath = null!, [CallerMemberName] string callerMethod = null!)
    {
        await VerifyHtml(htmlPath);

        await VerifyImage(imgPath, callerFilePath, callerMethod);
    }

    public async Task VerifyHtml(string htmlPath)
    {
        var html = await File.ReadAllTextAsync(htmlPath);
        await Verify(html, "html");
        File.Delete(htmlPath);
    }

    public async Task VerifyImage(string imgPath,
        [CallerFilePath] string callerFilePath = null!, [CallerMemberName] string callerMethod = null!)
    {
        var methodName = GetMethodName(callerFilePath, callerMethod);
        var destPath = Path.Combine(_snapshotsPath, $"{methodName}.png");
        var isEqual = await ImageComparer.IsEqualAsync(imgPath, destPath);
        Assert.True(isEqual, "Images are not equal.");
        File.Delete(imgPath);
    }
    #endregion

    #region Utils
    private static (string htmlPath, string imgPath) GetHtmlAndImagePath([CallerFilePath] string callerFilePath = null!, [CallerMemberName] string callerMethod = null!)
    {
        var methodName = GetMethodName(callerFilePath, callerMethod);

        var htmlPath = Path.Combine(_reportsPath, $"{methodName}.html");
        var imgPath = Path.Combine(_reportsPath, $"{methodName}.png");

        return (htmlPath, imgPath);
    }

    private static string GetMethodName(string callerFilePath, string callerMethod)
    {
        var callerTypeName = Path.GetFileNameWithoutExtension(callerFilePath);
        return $"{callerTypeName}.{callerMethod}";
    }
    #endregion
}