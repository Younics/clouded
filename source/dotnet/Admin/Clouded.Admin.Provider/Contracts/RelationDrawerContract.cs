using Clouded.Admin.Provider.Contexts;
using Clouded.Admin.Provider.Options;
using Clouded.Core.DataSource.Shared;

namespace Clouded.Admin.Provider.Contracts;

public class RelationDrawerContract
{
    public required string ComponentId { get; init; }
    public required bool IsOpen { get; set; }
    public required string Name { get; init; }

    public required TableOptions Table { get; init; }

    public required AdminContext Context { get; init; }

    public required IEnumerable<ColumnResult> Columns { get; init; }

    public required IEnumerable<RelationResult>? InsideRelations { get; init; }
    public DataSourceDictionary? PrefillEntity { get; set; }
    public string? PrefillEntityParam { get; set; }
}
