using Clouded.Shared.Enums;

namespace Clouded.Shared;

public static class EnumExtensions
{
    public static string GetEnumName<TEnum>(this TEnum @enum)
        where TEnum : struct, IComparable, IConvertible, IFormattable =>
        Enum.GetName(typeof(TEnum), @enum)!;

    public static TEnum AsEnum<TEnum>(this string enumName, bool ignoreCase = false)
        where TEnum : struct, IComparable, IConvertible, IFormattable =>
        Enum.Parse<TEnum>(enumName, ignoreCase);

    public static string[] GetEnumNames<TEnum>()
        where TEnum : struct, IComparable, IConvertible, IFormattable =>
        Enum.GetNames(typeof(TEnum));

    public static RepositoryType ToRepositoryType(this ERepositoryProvider provider)
    {
        return provider switch
        {
            ERepositoryProvider.Github => RepositoryType.Github,
            ERepositoryProvider.Gitlab => RepositoryType.Gitlab,
            ERepositoryProvider.Bitbucket => RepositoryType.BitBucket,
        };
    }

    public static ERepositoryProvider ToRepositoryProvider(this RepositoryType type)
    {
        return type switch
        {
            RepositoryType.Github => ERepositoryProvider.Github,
            RepositoryType.Gitlab => ERepositoryProvider.Gitlab,
            RepositoryType.BitBucket => ERepositoryProvider.Bitbucket
        };
    }
}
