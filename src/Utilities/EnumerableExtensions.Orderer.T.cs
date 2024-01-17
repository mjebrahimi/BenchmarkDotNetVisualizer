using System.Reflection;

namespace BenchmarkDotNetVisualizer.Utilities;

public static partial class EnumerableExtensions
{
#pragma warning disable S4136 // Method overloads should be grouped together
    #region Ascending
    public static IOrderedEnumerable<T> OrderByMultipleAuto<T>(this IEnumerable<T> enumerable, params string[] orderProperties)
    {
        return OrderByMultipleAuto(enumerable, orderProperties.ToDictionary(p => p, _ => OrderMode.Ascending));
    }

    public static IOrderedEnumerable<T> OrderByMultipleAuto<T>(this IEnumerable<T> enumerable, params Func<T, object?>[] orderProperties)
    {
        return OrderByMultipleAuto(enumerable, orderProperties.ToDictionary(p => p, _ => OrderMode.Ascending));
    }

    public static IOrderedEnumerable<T> OrderByMultiple<T>(this IEnumerable<T> enumerable, params string[] orderProperties)
    {
        return OrderByMultipleAuto(enumerable, orderProperties.ToDictionary(p => p, _ => OrderMode.Ascending));
    }

    public static IOrderedEnumerable<T> OrderByMultiple<T>(this IEnumerable<T> enumerable, params Func<T, object?>[] orderProperties)
    {
        return OrderByMultipleAuto(enumerable, orderProperties.ToDictionary(p => p, _ => OrderMode.Ascending));
    }

    public static IOrderedEnumerable<T> OrderByMultipleAsNumber<T>(this IEnumerable<T> enumerable, params string[] orderProperties)
    {
        return OrderByMultipleAuto(enumerable, orderProperties.ToDictionary(p => p, _ => OrderMode.Ascending));
    }

    public static IOrderedEnumerable<T> OrderByMultipleAsNumber<T>(this IEnumerable<T> enumerable, params Func<T, object?>[] orderProperties)
    {
        return OrderByMultipleAuto(enumerable, orderProperties.ToDictionary(p => p, _ => OrderMode.Ascending));
    }
    #endregion

    #region Descending

    public static IOrderedEnumerable<T> OrderByMultipleAutoDescending<T>(this IEnumerable<T> enumerable, params string[] orderProperties)
    {
        return OrderByMultipleAuto(enumerable, orderProperties.ToDictionary(p => p, _ => OrderMode.Descending));
    }

    public static IOrderedEnumerable<T> OrderByMultipleAutoDescending<T>(this IEnumerable<T> enumerable, params Func<T, object?>[] orderProperties)
    {
        return OrderByMultipleAuto(enumerable, orderProperties.ToDictionary(p => p, _ => OrderMode.Descending));
    }

    public static IOrderedEnumerable<T> OrderByMultipleDescending<T>(this IEnumerable<T> enumerable, params string[] orderProperties)
    {
        return OrderByMultipleAuto(enumerable, orderProperties.ToDictionary(p => p, _ => OrderMode.Descending));
    }

    public static IOrderedEnumerable<T> OrderByMultipleDescending<T>(this IEnumerable<T> enumerable, params Func<T, object?>[] orderProperties)
    {
        return OrderByMultipleAuto(enumerable, orderProperties.ToDictionary(p => p, _ => OrderMode.Descending));
    }

    public static IOrderedEnumerable<T> OrderByMultipleAsNumberDescending<T>(this IEnumerable<T> enumerable, params string[] orderProperties)
    {
        return OrderByMultipleAuto(enumerable, orderProperties.ToDictionary(p => p, _ => OrderMode.Descending));
    }

    public static IOrderedEnumerable<T> OrderByMultipleAsNumberDescending<T>(this IEnumerable<T> enumerable, params Func<T, object?>[] orderProperties)
    {
        return OrderByMultipleAuto(enumerable, orderProperties.ToDictionary(p => p, _ => OrderMode.Descending));
    }
    #endregion

    #region IDictionary

    public static IOrderedEnumerable<T> OrderByMultipleAuto<T>(this IEnumerable<T> enumerable, IDictionary<string, OrderMode> orderProperties)
    {
        ArgumentNullException.ThrowIfNull(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(orderProperties, nameof(orderProperties));

        var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

        IOrderedEnumerable<T> orderdItems = null!;
        for (int j = 0; j < orderProperties.Count; j++)
        {
            var orderProperty = orderProperties.ElementAt(j);

            switch (orderProperty.Value)
            {
                case OrderMode.Ascending:
                    orderdItems = j == 0 ?
                        enumerable.OrderBy(item => Array.Find(props, prop => prop.Name == orderProperty.Key)?.GetValue(item)?.GetValueForAutoOrdering()) :
                        orderdItems.ThenBy(item => Array.Find(props, prop => prop.Name == orderProperty.Key)?.GetValue(item)?.GetValueForAutoOrdering());
                    break;
                case OrderMode.Descending:
                    orderdItems = j == 0 ?
                        enumerable.OrderByDescending(item => Array.Find(props, prop => prop.Name == orderProperty.Key)?.GetValue(item)?.GetValueForAutoOrdering()) :
                        orderdItems.ThenByDescending(item => Array.Find(props, prop => prop.Name == orderProperty.Key)?.GetValue(item)?.GetValueForAutoOrdering());
                    break;
            }
        }
        return orderdItems;
    }

    public static IOrderedEnumerable<T> OrderByMultipleAuto<T>(this IEnumerable<T> enumerable, IDictionary<Func<T, object?>, OrderMode> orderProperties)
    {
        ArgumentNullException.ThrowIfNull(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(orderProperties, nameof(orderProperties));

        IOrderedEnumerable<T> orderdItems = null!;
        for (int j = 0; j < orderProperties.Count; j++)
        {
            var orderProperty = orderProperties.ElementAt(j);

            switch (orderProperty.Value)
            {
                case OrderMode.Ascending:
                    orderdItems = j == 0 ?
                        enumerable.OrderBy(item => orderProperty.Key(item)?.GetValueForAutoOrdering()) :
                        orderdItems.ThenBy(item => orderProperty.Key(item)?.GetValueForAutoOrdering());
                    break;
                case OrderMode.Descending:
                    orderdItems = j == 0 ?
                        enumerable.OrderByDescending(item => orderProperty.Key(item)?.GetValueForAutoOrdering()) :
                        orderdItems.ThenByDescending(item => orderProperty.Key(item)?.GetValueForAutoOrdering());
                    break;
            }
        }
        return orderdItems;
    }

    public static IOrderedEnumerable<T> OrderByMultiple<T>(this IEnumerable<T> enumerable, IDictionary<string, OrderMode> orderProperties)
    {
        ArgumentNullException.ThrowIfNull(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(orderProperties, nameof(orderProperties));

        var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

        IOrderedEnumerable<T> orderdItems = null!;
        for (int j = 0; j < orderProperties.Count; j++)
        {
            var orderProperty = orderProperties.ElementAt(j);

            switch (orderProperty.Value)
            {
                case OrderMode.Ascending:
                    orderdItems = j == 0 ?
                        enumerable.OrderBy(item => Array.Find(props, prop => prop.Name == orderProperty.Key)?.GetValue(item)) :
                        orderdItems.ThenBy(item => Array.Find(props, prop => prop.Name == orderProperty.Key)?.GetValue(item));
                    break;
                case OrderMode.Descending:
                    orderdItems = j == 0 ?
                        enumerable.OrderByDescending(item => Array.Find(props, prop => prop.Name == orderProperty.Key)?.GetValue(item)) :
                        orderdItems.ThenByDescending(item => Array.Find(props, prop => prop.Name == orderProperty.Key)?.GetValue(item));
                    break;
            }
        }
        return orderdItems;
    }

    public static IOrderedEnumerable<T> OrderByMultiple<T>(this IEnumerable<T> enumerable, IDictionary<Func<T, object?>, OrderMode> orderProperties)
    {
        ArgumentNullException.ThrowIfNull(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(orderProperties, nameof(orderProperties));

        IOrderedEnumerable<T> orderdItems = null!;
        for (int j = 0; j < orderProperties.Count; j++)
        {
            var orderProperty = orderProperties.ElementAt(j);

            switch (orderProperty.Value)
            {
                case OrderMode.Ascending:
                    orderdItems = j == 0 ?
                        enumerable.OrderBy(orderProperty.Key) :
                        orderdItems.ThenBy(orderProperty.Key);
                    break;
                case OrderMode.Descending:
                    orderdItems = j == 0 ?
                        enumerable.OrderByDescending(orderProperty.Key) :
                        orderdItems.ThenByDescending(orderProperty.Key);
                    break;
            }
        }
        return orderdItems;
    }

    public static IOrderedEnumerable<T> OrderByMultipleAsNumber<T>(this IEnumerable<T> enumerable, IDictionary<string, OrderMode> orderProperties)
    {
        ArgumentNullException.ThrowIfNull(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(orderProperties, nameof(orderProperties));

        var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

        IOrderedEnumerable<T> orderdItems = null!;
        for (int j = 0; j < orderProperties.Count; j++)
        {
            var orderProperty = orderProperties.ElementAt(j);

            switch (orderProperty.Value)
            {
                case OrderMode.Ascending:
                    orderdItems = j == 0 ?
                        enumerable.OrderBy(item => Array.Find(props, prop => prop.Name == orderProperty.Key)?.GetValue(item)?.ToString()?.ExtractNumber()) :
                        orderdItems!.ThenBy(item => Array.Find(props, prop => prop.Name == orderProperty.Key)?.GetValue(item)?.ToString()?.ExtractNumber());
                    break;
                case OrderMode.Descending:
                    orderdItems = j == 0 ?
                        enumerable.OrderByDescending(item => Array.Find(props, prop => prop.Name == orderProperty.Key)?.GetValue(item)?.ToString()?.ExtractNumber()) :
                        orderdItems!.ThenByDescending(item => Array.Find(props, prop => prop.Name == orderProperty.Key)?.GetValue(item)?.ToString()?.ExtractNumber());
                    break;
            }
        }
        return orderdItems;
    }

    public static IOrderedEnumerable<T> OrderByMultipleAsNumber<T>(this IEnumerable<T> enumerable, IDictionary<Func<T, object?>, OrderMode> orderProperties)
    {
        ArgumentNullException.ThrowIfNull(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(orderProperties, nameof(orderProperties));

        IOrderedEnumerable<T> orderdItems = null!;
        for (int j = 0; j < orderProperties.Count; j++)
        {
            var orderProperty = orderProperties.ElementAt(j);

            switch (orderProperty.Value)
            {
                case OrderMode.Ascending:
                    orderdItems = j == 0 ?
                        enumerable.OrderBy(item => orderProperty.Key(item)?.ToString()?.ExtractNumber()) :
                        orderdItems!.ThenBy(item => orderProperty.Key(item)?.ToString()?.ExtractNumber());
                    break;
                case OrderMode.Descending:
                    orderdItems = j == 0 ?
                        enumerable.OrderByDescending(item => orderProperty.Key(item)?.ToString()?.ExtractNumber()) :
                        orderdItems!.ThenByDescending(item => orderProperty.Key(item)?.ToString()?.ExtractNumber());
                    break;
            }
        }
        return orderdItems;
    }
    #endregion
#pragma warning restore S4136 // Method overloads should be grouped together
}