namespace Clouded.Platform.App.Web.Dtos;

public class AuthProviderIdentityInput
{
    public long? Id { get; set; }
    public bool Enabled { get; set; }
    public string? Schema { get; set; }
    public string? Table { get; set; }
    public string? ColumnId { get; set; }
    public string? ColumnIdentity { get; set; }
}
