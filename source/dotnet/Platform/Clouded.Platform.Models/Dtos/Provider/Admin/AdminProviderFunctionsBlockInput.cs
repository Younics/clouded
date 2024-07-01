namespace Clouded.Platform.Models.Dtos.Provider.Admin;

public class AdminProviderFunctionsBlockInput
{
    public IEnumerable<long> ValidationHooks { get; set; } = new List<long>();

    public IEnumerable<long> BeforeHooks { get; set; } = new List<long>();

    public IEnumerable<long> InputHooks { get; set; } = new List<long>();

    public IEnumerable<long> AfterHooks { get; set; } = new List<long>();
}
