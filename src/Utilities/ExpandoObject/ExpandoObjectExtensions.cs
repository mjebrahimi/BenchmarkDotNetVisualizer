// Ignore Spelling: expando

using System.Dynamic;
using System.Reflection;

namespace BenchmarkDotNetVisualizer.Utilities;

/// <summary>
/// ExpandoObject Extensions
/// </summary>
public static partial class ExpandoObjectExtensions
{
    /// <summary>
    /// Converts <typeparamref name="T"/> object to <see cref="ExpandoObject"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The object.</param>
    /// <returns></returns>
    public static ExpandoObject ToExpandoObject<T>(T obj)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));

        var expando = new ExpandoObject();
        var dictionary = expando.AsDictionary()!;

        foreach (var property in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            dictionary.Add(property.Name, property.GetValue(obj));

        return expando;
    }

    /// <summary>
    /// Casts the expando object as dynamic.
    /// </summary>
    /// <param name="expando">The expando.</param>
    /// <returns></returns>
    public static dynamic AsDynamic(ExpandoObject expando)
    {
        ArgumentNullException.ThrowIfNull(expando, nameof(expando));
        return expando;
    }

    /// <summary>
    /// Converts to <see cref="ExpandoObject"/> to typed <typeparamref name="T"/> object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="expando">The expando.</param>
    /// <returns></returns>
    public static T ToTypedObject<T>(this ExpandoObject expando) where T : new()
    {
        ArgumentNullException.ThrowIfNull(expando, nameof(expando));

        var type = typeof(T);
        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

        var instance = new T();

        foreach (var item in expando)
        {
            var propertyName = item.Key;
            var propertyValue = item.Value;

            var property = Array.Find(properties, p => p.Name == propertyName);

            property?.SetValue(instance, propertyValue);
        }

        return instance;
    }

    /// <summary>
    /// Determines whether the specified property name has property.
    /// </summary>
    /// <param name="expando">The expando.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>
    ///   <c>true</c> if the specified property name has property; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasProperty(this ExpandoObject expando, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(expando, nameof(expando));
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName, nameof(propertyName));

        var dictionary = expando.AsDictionary()!;
        return dictionary.ContainsKey(propertyName);
    }

    /// <summary>
    /// Sets the property.
    /// </summary>
    /// <param name="expando">The expando.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="propertyValue">The property value.</param>
    public static void SetProperty(this ExpandoObject expando, string propertyName, object? propertyValue)
    {
        ArgumentNullException.ThrowIfNull(expando, nameof(expando));
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName, nameof(propertyName));

        var dictionary = expando.AsDictionary()!;
        dictionary[propertyName] = propertyValue;
    }

    /// <summary>
    /// Casts the expando object as <![CDATA[IDictionary<string, object?>]]>.
    /// </summary>
    /// <param name="expando">The expando.</param>
    /// <returns></returns>
    public static IDictionary<string, object?> AsDictionary(this ExpandoObject expando) => expando;

    /// <summary>
    /// Gets the property value.
    /// </summary>
    /// <param name="expando">The expando.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    public static object? GetProperty(this ExpandoObject expando, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(expando, nameof(expando));
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName, nameof(propertyName));

        var dictionary = expando.AsDictionary()!;
        return dictionary[propertyName];
    }

    /// <summary>
    /// Gets the property value casts it as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="expando">The expando.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    public static T? GetProperty<T>(this ExpandoObject expando, string propertyName)
    {
        return (T?)expando.GetProperty(propertyName);
    }

    /// <summary>
    /// Tries to get the property value.
    /// </summary>
    /// <param name="expando">The expando.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static bool TryGetProperty(this ExpandoObject expando, string propertyName, out object? value)
    {
        ArgumentNullException.ThrowIfNull(expando, nameof(expando));
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName, nameof(propertyName));

        var dictionary = expando.AsDictionary()!;
        return dictionary.TryGetValue(propertyName, out value);
    }

    /// <summary>
    /// Tries to get the property value and cast as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="expando">The expando.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static bool TryGetProperty<T>(this ExpandoObject expando, string propertyName, out T? value)
    {
        var result = expando.TryGetProperty(propertyName, out var objValue);
        value = (T?)objValue;
        return result;
    }

    /// <summary>
    /// Gets the property value or default.
    /// </summary>
    /// <param name="expando">The expando.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    public static object? GetPropertyOrDefault(this ExpandoObject expando, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(expando, nameof(expando));
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName, nameof(propertyName));

        var dictionary = expando.AsDictionary()!;
        dictionary.TryGetValue(propertyName, out var value);
        return value;
    }

    /// <summary>
    /// Gets the property value as <typeparamref name="T"/> or default.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="expando">The expando.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    public static T? GetPropertyOrDefault<T>(this ExpandoObject expando, string propertyName)
    {
        return (T?)expando.GetPropertyOrDefault(propertyName);
    }

    /// <summary>
    /// Changes the name of the property.
    /// </summary>
    /// <param name="expando">The expando.</param>
    /// <param name="fromPropertyName">Name of from property.</param>
    /// <param name="toPropertyName">Name of to property.</param>
    public static void ChangePropertyName(this ExpandoObject expando, string fromPropertyName, string toPropertyName)
    {
        ArgumentNullException.ThrowIfNull(expando, nameof(expando));
        ArgumentException.ThrowIfNullOrWhiteSpace(fromPropertyName, nameof(fromPropertyName));
        ArgumentException.ThrowIfNullOrWhiteSpace(toPropertyName, nameof(toPropertyName));

        if (fromPropertyName == toPropertyName)
            return;

        var dictionary = expando.AsDictionary()!;
        dictionary[toPropertyName] = dictionary[fromPropertyName];
        dictionary.Remove(fromPropertyName);
    }

    /// <summary>
    /// Removes the specified property.
    /// </summary>
    /// <param name="expando">The expando.</param>
    /// <param name="propertyName">Name of the property.</param>
    public static void RemoveProperty(this ExpandoObject expando, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(expando, nameof(expando));
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName, nameof(propertyName));

        var dictionary = expando.AsDictionary()!;
        dictionary.Remove(propertyName);
    }

    /// <summary>
    /// Removes the specified properties.
    /// </summary>
    /// <param name="expando">The expando.</param>
    /// <param name="propertyNames">The property names.</param>
    public static void RemoveProperties(this ExpandoObject expando, params string[] propertyNames)
    {
        ArgumentNullException.ThrowIfNull(expando, nameof(expando));
        Guard.ThrowIfNullOrEmpty(propertyNames, nameof(propertyNames));

        var dictionary = expando.AsDictionary()!;
        foreach (var propertyName in propertyNames)
            dictionary.Remove(propertyName);
    }

    /// <summary>
    /// Removes the specified properties.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="propertyNames">The property names.</param>
    public static void RemoveProperties(this IEnumerable<ExpandoObject?> enumerable, params string[] propertyNames)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        ArgumentNullException.ThrowIfNull(propertyNames, nameof(propertyNames));

        foreach (var expando in enumerable)
        {
            if (expando is null)
                continue;

#pragma warning disable IDE0305 // Simplify collection initialization
            var propsToRemove = propertyNames.Length > 0 ? propertyNames : expando.AsDictionary()!.Keys.ToArray();
#pragma warning restore IDE0305 // Simplify collection initialization
            expando.RemoveProperties(propsToRemove);
        }
    }

    /// <summary>
    /// Removes properties except the specified properties.
    /// </summary>
    /// <param name="expando">The expando.</param>
    /// <param name="propertyNames">The property names.</param>
    public static void RemovePropertiesExcept(this ExpandoObject expando, params string[] propertyNames)
    {
        ArgumentNullException.ThrowIfNull(expando, nameof(expando));
        Guard.ThrowIfNullOrEmpty(propertyNames, nameof(propertyNames));

        var dictionary = expando.AsDictionary()!;
        foreach (var propertyName in dictionary.Keys.Except(propertyNames).ToArray())
            dictionary.Remove(propertyName);
    }

    /// <summary>
    /// Removes properties except the specified properties.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="propertyNames">The property names.</param>
    public static void RemovePropertiesExcept(this IEnumerable<ExpandoObject?> enumerable, params string[] propertyNames)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        ArgumentNullException.ThrowIfNull(propertyNames, nameof(propertyNames));

        foreach (var expando in enumerable)
        {
            if (expando is null)
                continue;

#pragma warning disable IDE0305 // Simplify collection initialization
            var propsToRemove = propertyNames.Length > 0 ? propertyNames : expando.AsDictionary()!.Keys.ToArray();
#pragma warning restore IDE0305 // Simplify collection initialization
            expando.RemovePropertiesExcept(propsToRemove);
        }
    }
}
