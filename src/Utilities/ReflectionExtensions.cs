using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BenchmarkDotNetVisualizer.Utilities;

/// <summary>
/// Reflection Extensions
/// </summary>
public static class ReflectionExtensions
{
    /// <summary>
    /// Gets the attribute property.
    /// </summary>
    /// <typeparam name="TAttr">The type of the attribute.</typeparam>
    /// <typeparam name="TProp">The type of the property.</typeparam>
    /// <param name="type">The type.</param>
    /// <param name="propertySelector">The property selector.</param>
    /// <param name="inherit">Set <c>true</c> to inspect the ancestors of element; otherwise, <c>false</c>.</param>
    /// <returns></returns>
    public static TProp GetAttributeProperty<TAttr, TProp>(this Type type, Func<TAttr?, TProp> propertySelector, bool inherit = false) where TAttr : Attribute
    {
        var attr = type.GetCustomAttribute<TAttr>(inherit);
        return propertySelector(attr);
    }

    /// <summary>
    /// Gets the <see cref="DisplayAttribute.GroupName"/> value of the type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    public static string? GetGroupName(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));

        return type.GetCustomAttribute<DisplayAttribute>()?.GroupName;
    }

    /// <summary>
    /// Gets the <see cref="DisplayAttribute.Name"/> or <see cref="DisplayNameAttribute.DisplayName"/> value of the type or fallback to the Type.Name.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="fallbackToTypeName">if set to <c>true</c> fallback to Type.Name.</param>
    /// <param name="format">if set to <c>true</c> [format].</param>
    /// <returns></returns>
    public static string? GetDisplayName(this Type type, bool fallbackToTypeName = true, bool format = true)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));

        var benchmarkDisplayName =
                type.GetCustomAttribute<DisplayAttribute>()?.Name
                ?? type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;

        if (fallbackToTypeName)
            benchmarkDisplayName ??= type.Name;

        if (benchmarkDisplayName is not null && format)
        {
            var genericTypeNames = type.GenericTypeArguments.Select(p => p.Name).ToArray();
            benchmarkDisplayName = string.Format(benchmarkDisplayName, genericTypeNames);
        }

        return benchmarkDisplayName;
    }

    /// <summary>
    /// Determines whether the specified type is numeric type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>
    ///   <c>true</c> if the specified type is numeric type; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNumericType(this Type type)
    {
        return type == typeof(byte) ||
               type == typeof(sbyte) ||
               type == typeof(ushort) ||
               type == typeof(uint) ||
               type == typeof(ulong) ||
               type == typeof(short) ||
               type == typeof(int) ||
               type == typeof(long) ||
               type == typeof(decimal) ||
               type == typeof(double) ||
               type == typeof(float);
    }
}
