using Clouded.Core.DataSource.Shared.Enums;
using Clouded.Core.DataSource.Shared.Input;
using Clouded.Shared.Enums;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace Clouded.Core.DataSource.Shared;

public static class ColumnHelper
{
    public static IEnumerable<ConditionValueInput> Where(
        string alias,
        Dictionary<string, StringValues> primaryKeys,
        IEnumerable<ColumnResult> columns
    ) =>
        primaryKeys.Select(pair =>
        {
            var type = columns.FirstOrDefault(x => x.Name == pair.Key)?.Type;

            return new ConditionValueInput
            {
                Alias = alias,
                Column = pair.Key,
                Operator = EOperator.Equals,
                Value = ResolveColumnValue<object>(type, pair.Value.ToString())
            };
        });

    public static IEnumerable<ConditionValueInput> WherePk(
        string alias,
        Dictionary<string, string> primaryKeys,
        IEnumerable<ColumnResult> columns
    ) =>
        primaryKeys.Select(pair =>
        {
            var type = columns.FirstOrDefault(x => x.Name == pair.Key)?.Type;

            return new ConditionValueInput
            {
                Alias = alias,
                Column = pair.Key,
                Operator = EOperator.Equals,
                Value = ResolveColumnValue<object>(type, pair.Value)
            };
        });

    public static T? ResolveColumnValue<T>(EColumnType? type, object? value)
    {
        value = type switch
        {
            EColumnType.Boolean => Convert.ToBoolean(value),
            EColumnType.Char => Convert.ToChar(value),
            EColumnType.Varchar or EColumnType.Text => Convert.ToString(value),
            EColumnType.Int => Convert.ToInt32(value),
            EColumnType.Long => Convert.ToInt64(value),
            EColumnType.Double => Convert.ToSingle(value),
            EColumnType.Decimal => Convert.ToDecimal(value),
            EColumnType.Time
                => value == null
                    ? TimeSpan.Zero
                    : TimeSpan.Parse(value?.ToString() ?? string.Empty),
            EColumnType.Date
            or EColumnType.DateTime
                => value == null ? null : Convert.ToDateTime(value),
            _ => value?.ToString()
        };

        return (T?)value;
    }

    public static Dictionary<string, object?> GetPrimaryColumns(
        this DataSourceDictionary entity,
        IEnumerable<ColumnResult> columns
    )
    {
        return columns.Where(x => x.IsPrimary).ToDictionary(x => x.Name, x => entity[x.Name]);
    }

    public static Dictionary<string, string> GetPrimaryColumnsToString(
        this DataSourceDictionary entity,
        IEnumerable<ColumnResult> columns
    )
    {
        return columns
            .Where(x => x.IsPrimary && entity[x.Name] != null)
            .ToDictionary(x => x.Name, x => entity[x.Name]!.ToString())!;
    }

    public static string ToEntityDetailUri(
        this DataSourceDictionary entity,
        IEnumerable<ColumnResult> columns,
        string baseUri
    )
    {
        return QueryHelpers.AddQueryString(
            baseUri,
            entity.GetPrimaryColumns(columns).ToDictionary(x => x.Key, x => x.Value.ToString())
        );
    }

    public static string ToImageSrc(object? image)
    {
        if (image == null)
        {
            return "";
        }

        if (image is string value)
        {
            image = File.ReadAllBytes(value);
        }

        var imageSrc = Convert.ToBase64String((byte[])image);
        return $"data:image/jpeg;base64,{imageSrc}";
    }

    public static string GetKey(this ColumnResult column)
    {
        return $"{column.Name}.{column.TypeRaw}";
    }
}
