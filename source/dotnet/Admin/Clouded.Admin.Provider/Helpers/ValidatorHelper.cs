using Clouded.Admin.Provider.Options;
using Clouded.Core.DataSource.Shared;
using Clouded.Shared.Enums;
using FluentValidation;

namespace Clouded.Admin.Provider.Helpers;

public static class ValidatorHelper
{
    public static FluentValueValidator<string?> StringValueValidator(
        this FluentValueValidator<string?> validator,
        ColumnResult column,
        TableColumnOptions columnOptions
    )
    {
        if (!column.IsNullable)
        {
            validator.RuleFor(x => x).NotEmpty().WithName(columnOptions.Name);
        }

        if (column.MaxLength != null)
        {
            validator
                .RuleFor(x => x)
                .MaximumLength((int)column.MaxLength)
                .WithName(columnOptions.Name);
        }

        if (columnOptions.Type == ETableColumnType.Email)
        {
            validator.RuleFor(x => x).EmailAddress().WithName(columnOptions.Name);
        }

        switch (columnOptions.ValidationType)
        {
            case TableColumnValidation.Regex:
                validator
                    .RuleFor(x => x)
                    .Matches(columnOptions.ValidationRegex)
                    .WithName(columnOptions.Name);
                break;
            case TableColumnValidation.CreditCard:
                validator.RuleFor(x => x).CreditCard().WithName(columnOptions.Name);
                break;
            case null:
                break;
        }

        return validator;
    }

    public static FluentValueValidator<float?> NumericValueValidator(
        this FluentValueValidator<float?> validator,
        ColumnResult column,
        TableColumnOptions columnOptions
    )
    {
        if (!column.IsNullable)
        {
            validator.RuleFor(x => x).NotEmpty().WithName(columnOptions.Name);
        }

        switch (columnOptions.ValidationType)
        {
            case TableColumnValidation.LessThan:
                validator
                    .RuleFor(x => x)
                    .LessThan((float)columnOptions.ValidationValue)
                    .WithName(columnOptions.Name);
                break;
            case TableColumnValidation.GreaterThan:
                validator
                    .RuleFor(x => x)
                    .GreaterThan((float)columnOptions.ValidationValue)
                    .WithName(columnOptions.Name);
                break;
            case TableColumnValidation.LessThanOrEqual:
                validator
                    .RuleFor(x => x)
                    .LessThanOrEqualTo((float)columnOptions.ValidationValue)
                    .WithName(columnOptions.Name);
                break;
            case TableColumnValidation.GreaterThanOrEqual:
                validator
                    .RuleFor(x => x)
                    .GreaterThanOrEqualTo((float)columnOptions.ValidationValue)
                    .WithName(columnOptions.Name);
                break;
            case null:
                break;
        }

        return validator;
    }

    public static FluentValueValidator<DataSourceDictionary?> ObjectValueValidator(
        this FluentValueValidator<DataSourceDictionary?> validator,
        ColumnResult column,
        TableColumnOptions columnOptions
    )
    {
        if (!column.IsNullable)
        {
            validator.RuleFor(x => x).NotEmpty().WithName(columnOptions.Name);
        }

        return validator;
    }

    public static FluentValueValidator<DataSourceDictionary?> RequiredObjectValueValidator(
        this FluentValueValidator<DataSourceDictionary?> validator
    )
    {
        validator.RuleFor(x => x).NotEmpty();

        return validator;
    }
}
