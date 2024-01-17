using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace BenchmarkDotNetVisualizer;

/// <summary>
/// CustomTagColumn
/// </summary>
/// <seealso cref="IColumn" />
public class CustomTagColumn : IColumn
{
    private readonly Func<Summary, BenchmarkCase, string>? getValue1;
    private readonly Func<Summary, BenchmarkCase, SummaryStyle, string>? getValue2;

    /// <inheritdoc/>
    public string Id { get; }

    /// <inheritdoc/>
    public string ColumnName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomTagColumn"/> class.
    /// </summary>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="getValue">The get value function.</param>
    public CustomTagColumn(string columnName, Func<Summary, BenchmarkCase, string> getValue)
    {
        getValue1 = getValue;
        ColumnName = columnName;
        Id = nameof(CustomTagColumn) + "." + ColumnName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomTagColumn"/> class.
    /// </summary>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="getValue">The get value function.</param>
    public CustomTagColumn(string columnName, Func<Summary, BenchmarkCase, SummaryStyle, string> getValue)
    {
        getValue2 = getValue;
        ColumnName = columnName;
        Id = nameof(CustomTagColumn) + "." + ColumnName;
    }

    /// <inheritdoc/>
    public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;

    /// <inheritdoc/>
    public bool IsAvailable(Summary summary) => true;

    /// <inheritdoc/>
    public bool AlwaysShow => true;

    /// <inheritdoc/>
    public ColumnCategory Category => ColumnCategory.Custom;

    /// <inheritdoc/>
    public int PriorityInCategory => 0;

    /// <inheritdoc/>
    public bool IsNumeric => false;

    /// <inheritdoc/>
    public UnitType UnitType => UnitType.Dimensionless;

    /// <inheritdoc/>
    public string Legend => $"Custom '{ColumnName}' tag column";

    /// <inheritdoc/>
    public string GetValue(Summary summary, BenchmarkCase benchmarkCase) => getValue1!.Invoke(summary, benchmarkCase);

    /// <inheritdoc/>
    public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style) => getValue2?.Invoke(summary, benchmarkCase, style) ?? GetValue(summary, benchmarkCase);

    /// <inheritdoc/>
    public override string ToString() => ColumnName;
}