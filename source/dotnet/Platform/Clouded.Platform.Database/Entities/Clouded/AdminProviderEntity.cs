namespace Clouded.Platform.Database.Entities.Clouded;

public class AdminProviderEntity : ProviderEntity
{
    public virtual ICollection<DataSourceEntity> DataSources { get; set; } = null!;
    public virtual ICollection<AdminUserAccessConfigurationEntity> UserAccess { get; set; } = null!;
    public virtual ICollection<AdminNavigationGroupEntity> NavGroups { get; set; } = null!;
    public virtual ICollection<AdminTablesConfigurationEntity> Tables { get; set; } = null!;
    public virtual ICollection<AdminProviderDataSourceRelationEntity> DataSourcesRelation { get; set; } =
        null!;
    public virtual ICollection<FunctionEntity> Functions { get; set; } = null!;
    public virtual ICollection<AdminProviderFunctionRelationEntity> FunctionsRelation { get; set; } =
        null!;
    public virtual AdminConfigurationEntity Configuration { get; set; } = null!;
}
