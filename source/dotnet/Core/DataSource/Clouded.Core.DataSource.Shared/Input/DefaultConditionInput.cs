using Clouded.Core.DataSource.Shared.Interfaces;

namespace Clouded.Core.DataSource.Shared.Input;

public class DefaultConditionInput : ICondition
{
    public string Id => $"{$"DefaultConditionInput".GetHashCode():X}";
}
