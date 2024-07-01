using Clouded.Core.DataSource.Shared;

namespace Clouded.Admin.Provider.Contracts;

public class CreatedRelationContract
{
    public required string ComponentId { get; init; }
    public DataSourceDictionary CreatedEntity { get; init; } = null!;
}
