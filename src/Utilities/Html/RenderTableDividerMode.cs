namespace BenchmarkDotNetVisualizer;

/// <summary>
/// Render Table Divider Mode
/// </summary>
public enum RenderTableDividerMode
{
    /// <summary>
    /// Renders empty rows as null divider rows
    /// </summary>
    EmptyDividerRow,
    /// <summary>
    /// Ignores null divider rows when rendering
    /// </summary>
    Ignore,
    /// <summary>
    /// Separates tables with null divider rows when rendering
    /// </summary>
    SeparateTables
}
