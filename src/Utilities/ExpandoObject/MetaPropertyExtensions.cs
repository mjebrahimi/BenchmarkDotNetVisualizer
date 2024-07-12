using System.Dynamic;
using System.Runtime.CompilerServices;

namespace BenchmarkDotNetVisualizer.Utilities;

/// <summary>
/// MetaProperty Extensions
/// </summary>
public static class MetaPropertyExtensions
{
    private static readonly ConditionalWeakTable<object, ExpandoObject> conditionalWeakTable = [];

    /// <summary>
    /// Sets the meta property.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="propertyValue">The property value.</param>
    /// <returns></returns>
    public static void SetMetaProperty(this object obj, string propertyName, object? propertyValue)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName, nameof(propertyName));

        var expando = conditionalWeakTable.GetOrCreateValue(obj);
        expando.SetProperty(propertyName, propertyValue);
    }

    /// <summary>
    /// Sets the meta properties.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="expando">The expando.</param>
    /// <returns></returns>
    public static void SetMetaProperties(this object obj, ExpandoObject expando)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));
        ArgumentNullException.ThrowIfNull(expando, nameof(expando));

        conditionalWeakTable.AddOrUpdate(obj, expando);
    }

    /// <summary>
    /// Gets the meta property value.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    public static object? GetMetaProperty(this object obj, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName, nameof(propertyName));

        var expando = conditionalWeakTable.GetOrCreateValue(obj);
        return expando.GetProperty(propertyName);
    }

    /// <summary>
    /// Gets the meta property value and cast it as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The object.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    public static T? GetMetaProperty<T>(this object obj, string propertyName)
    {
        return (T?)obj.GetMetaProperty(propertyName);
    }

    /// <summary>
    /// Tries to get meta property value.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static bool TryGetMetaProperty(this object obj, string propertyName, out object? value)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName, nameof(propertyName));

        var expando = conditionalWeakTable.GetOrCreateValue(obj);
        return expando.TryGetProperty(propertyName, out value);
    }

    /// <summary>
    /// Tries the get meta property value and cast it as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The object.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static bool TryGetMetaProperty<T>(this object obj, string propertyName, out T? value)
    {
        var result = obj.TryGetMetaProperty(propertyName, out var objValue);
        value = (T?)objValue;
        return result;
    }

    /// <summary>
    /// Gets the meta property value or default.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    public static object? GetMetaPropertyOrDefault(this object obj, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName, nameof(propertyName));

        var expando = conditionalWeakTable.GetOrCreateValue(obj);
        return expando.GetPropertyOrDefault(propertyName);
    }

    /// <summary>
    /// Gets the meta property value and casts it as <typeparamref name="T"/> or default.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The object.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    public static T? GetMetaPropertyOrDefault<T>(this object obj, string propertyName)
    {
        return (T?)obj.GetMetaPropertyOrDefault(propertyName);
    }

    /// <summary>
    /// Gets the meta properties.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns></returns>
    public static ExpandoObject GetMetaProperties(this object obj)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));

        return conditionalWeakTable.GetOrCreateValue(obj);
    }

    /// <summary>
    /// Gets the meta properties and casts it as dynamic.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns></returns>
#pragma warning disable S4144 // Methods should not have identical implementations
    public static dynamic GetMetaPropertiesAsDynamic(this object obj)
#pragma warning restore S4144 // Methods should not have identical implementations
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));

        return conditionalWeakTable.GetOrCreateValue(obj);
    }

    /// <summary>
    /// Determines whether the specified object has meta property.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>
    ///   <c>true</c> if the specified object has meta property; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasMetaProperty(this object obj, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName, nameof(propertyName));

        var expando = conditionalWeakTable.GetOrCreateValue(obj);
        var dictionary = expando.AsDictionary()!;
        return dictionary.ContainsKey(propertyName);
    }

    /// <summary>
    /// Changes the name of the meta property.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="fromPropertyName">Name of from property.</param>
    /// <param name="toPropertyName">Name of to property.</param>
    /// <returns></returns>
    public static void ChangeMetaPropertyName(this object obj, string fromPropertyName, string toPropertyName)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));
        ArgumentException.ThrowIfNullOrWhiteSpace(fromPropertyName, nameof(fromPropertyName));
        ArgumentException.ThrowIfNullOrWhiteSpace(toPropertyName, nameof(toPropertyName));

        if (fromPropertyName == toPropertyName)
            return;

        var expando = conditionalWeakTable.GetOrCreateValue(obj);
        var dictionary = expando.AsDictionary()!;
        dictionary[toPropertyName] = dictionary[fromPropertyName];
        dictionary.Remove(fromPropertyName);
    }

    /// <summary>
    /// Removes the specified meta property.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    public static void RemoveMetaProperty(this object obj, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName, nameof(propertyName));

        var expando = conditionalWeakTable.GetOrCreateValue(obj);
        var dictionary = expando.AsDictionary()!;
        dictionary.Remove(propertyName);
    }

    /// <summary>
    /// Removes the specified meta properties.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="propertyNames">The property names.</param>
    /// <returns></returns>
    public static void RemoveMetaProperties(this object obj, params string[] propertyNames)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));
        Guard.ThrowIfNullOrEmpty(propertyNames, nameof(propertyNames));

        var expando = conditionalWeakTable.GetOrCreateValue(obj);
        var dictionary = expando.AsDictionary()!;
        foreach (var propertyName in propertyNames)
            dictionary.Remove(propertyName);
    }

    /// <summary>
    /// Removes meta properties except the specified properties.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="propertyNames">The property names.</param>
    /// <returns></returns>
    public static void RemoveMetaPropertiesExcept(this object obj, params string[] propertyNames)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));
        Guard.ThrowIfNullOrEmpty(propertyNames, nameof(propertyNames));

        var expando = conditionalWeakTable.GetOrCreateValue(obj);
        var dictionary = expando.AsDictionary()!;
        foreach (var propertyName in dictionary.Keys.Except(propertyNames).ToArray())
            dictionary.Remove(propertyName);
    }
}