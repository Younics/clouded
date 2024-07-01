using Clouded.Shared.Enums;

namespace Clouded.Platform.App.Web.Dtos;

public class FunctionProviderInput
{
    public long? Id { get; set; }
    public string Name { get; set; } = null!;
    public string CodePrefix { get; set; } = null!;
    public string RepositoryId { get; set; } = null!;
    public string Branch { get; set; } = null!;
    public RepositoryType RepositoryType { get; set; }
    public IEnumerable<FunctionProviderExecutableInput> Hooks { get; set; } =
        Array.Empty<FunctionProviderExecutableInput>();
    public long ProjectId { get; set; }
}
