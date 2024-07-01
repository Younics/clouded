namespace Clouded.Function.Framework.Outputs;

public sealed class ValidationOutput
{
    public bool Passed { get; init; }
    public int Code { get; init; }
    public String Message { get; init; } = null!;
    public String? I18N { get; init; }
}
