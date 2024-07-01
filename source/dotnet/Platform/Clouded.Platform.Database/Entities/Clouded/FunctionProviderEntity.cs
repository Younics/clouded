namespace Clouded.Platform.Database.Entities.Clouded;

public class FunctionProviderEntity : ProviderEntity
{
    public virtual FunctionConfigurationEntity Configuration { get; set; } = null!;

    public virtual ICollection<FunctionEntity> Functions { get; set; } = null!;
}
