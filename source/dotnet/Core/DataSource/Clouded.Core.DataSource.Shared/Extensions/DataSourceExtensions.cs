namespace Clouded.Core.DataSource.Shared.Extensions;

public static class DataSourceExtensions
{
    public static T ToDataSourceDictionary<T>(this Dictionary<string, object?> source)
        where T : DataSourceDictionary, new()
    {
        var dictionary = new T();

        foreach (var pair in source)
            dictionary.Add(pair.Key, pair.Value);

        return dictionary;
    }

    public static T ToDataSourceDictionary<T>(
        this IEnumerable<KeyValuePair<string, object?>> source
    )
        where T : DataSourceDictionary, new()
    {
        var dictionary = new T();

        foreach (var pair in source)
            dictionary.Add(pair.Key, pair.Value);

        return dictionary;
    }

    public static object? GetColumnValue(
        this DataSourceDictionary source,
        string column,
        object? fallbackValue = null
    )
    {
        if (source.TryGetValue(column, out object? value))
        {
            return value;
        }
        else
        {
            return fallbackValue ?? "";
        }
    }
}
