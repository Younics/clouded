namespace Clouded.Auth.Provider.Options;

public class IdentityAppleDataOptions
{
    public string Schema { get; set; } = null!;
    public readonly string Table = "clouded_auth_support_users_apple_data";
    public readonly string ColumnId = "id";
    public readonly string KeyPrefix = "cs_apple_data";
    public readonly string ColumnCode = "code";
    public readonly string ColumnEmail = "email";
    public readonly string ColumnFirstName = "first_name";
    public readonly string ColumnLastName = "last_name";
}
