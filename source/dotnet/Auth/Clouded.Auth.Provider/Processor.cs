using System.Collections;
using System.Text.Json;
using Clouded.Core.DataSource.Shared;
using Clouded.Shared.Enums;

namespace Clouded.Auth.Provider;

public static class Processor
{
    public static TDictionary Transform<TDictionary>(this TDictionary data)
        where TDictionary : DataSourceDictionary
    {
        foreach (var pair in data)
            data[pair.Key] = pair.Value.Transform();

        return data;
    }

    public static TDictionary Transform<TDictionary>(
        this TDictionary data,
        IEnumerable<ColumnResult> columns
    )
        where TDictionary : DataSourceDictionary
    {
        var columnsArray = columns.ToArray();

        foreach (var pair in data)
        {
            var column = columnsArray.FirstOrDefault(x => x.Name == pair.Key);

            data[pair.Key] =
                column != null ? pair.Value.Transform(column.Type) : pair.Value.Transform();
        }

        return data;
    }

    public static Dictionary<string, object?> Transform(this Dictionary<string, object?> data)
    {
        foreach (var pair in data)
            data[pair.Key] = pair.Value.Transform();

        return data;
    }

    public static IList Transform(this IEnumerable<object?> values)
    {
        var valuesTyped = values.Select(value => value.Transform()).ToArray();

        if (!valuesTyped.Any())
            return valuesTyped;

        var listType = typeof(List<>).MakeGenericType(valuesTyped.First()!.GetType());
        var list = (IList)Activator.CreateInstance(listType)!;

        foreach (var value in valuesTyped)
            list.Add(value);

        return list;
    }

    public static object? Transform(this object? value, EColumnType type)
    {
        if (value == null)
            return null;

        var parsedValue = value;

        if (value is JsonElement element)
            parsedValue = element.Transform();

        try
        {
            switch (type)
            {
                case EColumnType.Boolean:
                    return Convert.ToBoolean(parsedValue);
                case EColumnType.Char:
                    return Convert.ToChar(parsedValue);
                case EColumnType.Varchar:
                case EColumnType.Text:
                    return Convert.ToString(parsedValue);
                case EColumnType.Int:
                    return Convert.ToInt32(parsedValue);
                case EColumnType.Long:
                    return Convert.ToInt64(parsedValue);
                case EColumnType.Double:
                    return Convert.ToDouble(parsedValue);
                case EColumnType.Decimal:
                    return Convert.ToDecimal(parsedValue);
                case EColumnType.Time:
                    return Convert.ToSingle(parsedValue);
                case EColumnType.Date:
                case EColumnType.DateTime:
                    return Convert.ToDateTime(parsedValue);
                case EColumnType.SmallSerial:
                    return Convert.ToInt16(parsedValue);
                case EColumnType.Serial:
                    return Convert.ToInt32(parsedValue);
                case EColumnType.BigSerial:
                    return Convert.ToInt64(parsedValue);
                case EColumnType.Bytea:
                    return Convert.ToByte(parsedValue);
                default:
                    return parsedValue;
            }
        }
        catch
        {
            return parsedValue;
        }
    }

    public static object? Transform(this object? value)
    {
        if (value == null)
            return null;

        if (short.TryParse(value.ToString(), out var shortValue))
            return shortValue;
        if (int.TryParse(value.ToString(), out var intValue))
            return intValue;
        if (long.TryParse(value.ToString(), out var longValue))
            return longValue;
        if (double.TryParse(value.ToString(), out var doubleValue))
            return doubleValue;
        if (decimal.TryParse(value.ToString(), out var decimalValue))
            return decimalValue;

        if (value is not JsonElement element)
            return value;

        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                if (element.TryGetDateTimeOffset(out var dateTimeOffsetValue))
                    return dateTimeOffsetValue;
                if (element.TryGetDateTime(out var dateTimeValue))
                    return dateTimeValue;

                return element.GetString() ?? string.Empty;
            case JsonValueKind.True:
            case JsonValueKind.False:
                return element.GetBoolean();
            case JsonValueKind.Number:
                if (short.TryParse(value.ToString(), out shortValue))
                    return shortValue;
                if (int.TryParse(value.ToString(), out intValue))
                    return intValue;
                if (long.TryParse(value.ToString(), out longValue))
                    return longValue;
                if (double.TryParse(value.ToString(), out doubleValue))
                    return doubleValue;
                if (decimal.TryParse(value.ToString(), out decimalValue))
                    return decimalValue;
                break;
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                return null;
            case JsonValueKind.Object:
            case JsonValueKind.Array:
            default:
                return value;
        }

        return value;
    }
}
