using Clouded.Shared.Enums;

namespace Clouded.Platform.Models.Dtos.Provider.Admin;

public class AdminProviderTableInput
{
    public long? Id { get; set; }
    public long? AdminProviderId { get; set; }
    public long DataSourceId { get; set; }
    public string Schema { get; set; }
    public string Table { get; set; }
    public string Name { get; set; }
    public string? SingularName { get; set; }
    public bool InMenu { get; set; }
    public bool Enabled { get; set; }
    public string? NavGroup { get; set; }
    public string? Icon { get; set; }
    public List<AdminProviderTableColumnInput> Columns { get; set; } = new();
    public List<AdminProviderTableColumnInput> VirtualColumns { get; set; } = new();

    public AdminProviderFunctionsBlockInput CreateFunctions { get; set; } = new();
    public AdminProviderFunctionsBlockInput UpdateFunctions { get; set; } = new();
    public AdminProviderFunctionsBlockInput DeleteFunctions { get; set; } = new();

    public class AdminProviderTableColumnInput
    {
        public bool Enabled { get; set; }
        public bool Filterable { get; set; }
        public string Column { get; set; }
        public string Name { get; set; }
        public int Order { get; set; } = 1;
        public string? VirtualValue { get; set; }
        public EVirtualColumnType VirtualType { get; set; } = EVirtualColumnType.String;
        public ETableColumnType Type { get; set; } = ETableColumnType.Inherits;
        public AdminProviderTableColumnPermissionInput List { get; set; } = new();
        public AdminProviderTableColumnPermissionInput Detail { get; set; } = new();
        public AdminProviderTableColumnPermissionInput Create { get; set; } = new();
    }

    public class AdminProviderTableColumnPermissionInput
    {
        public bool Visible { get; set; } = true;
        public bool Readonly { get; set; }
    }
}
