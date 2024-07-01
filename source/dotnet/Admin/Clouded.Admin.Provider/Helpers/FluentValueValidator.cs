using Clouded.Core.DataSource.Shared;
using FluentValidation;

namespace Clouded.Admin.Provider.Helpers;

/// <summary>
/// A glue class to make it easy to define validation rules for single values using FluentValidation
/// You can reuse this class for all your fields, like for the credit card rules above.
/// </summary>
/// <typeparam name="T"></typeparam>
public class FluentValueValidator<T> : AbstractValidator<T>
{
    public FluentValueValidator() { }

    public FluentValueValidator(Action<IRuleBuilderInitial<T, T>> rule)
    {
        rule(RuleFor(x => x));
    }

    private IEnumerable<string> ValidateValue(T arg)
    {
        // this is for Relation, arg validator T == DataSourceDictionary
        if (arg == null && typeof(DataSourceDictionary).IsAssignableFrom(typeof(T)))
        {
            return new[] { "Field must not be empty." };
        }

        var result = Validate(arg);

        if (result.IsValid)
            return Array.Empty<string>();

        return result.Errors.Select(e => e.ErrorMessage);
    }

    public Func<T, IEnumerable<string>> Validation => ValidateValue;
}
