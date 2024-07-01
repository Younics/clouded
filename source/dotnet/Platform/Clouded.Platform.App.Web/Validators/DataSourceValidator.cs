using Clouded.Platform.App.Web.Dtos;
using FluentValidation;

namespace Clouded.Platform.App.Web.Validators;

public class DataSourceValidator : AbstractValidator<DataSourceInput>
{
    public DataSourceValidator()
    {
        RuleFor(entity => entity.Name).NotEmpty();
        RuleFor(entity => entity.Type).NotEmpty();
        RuleFor(entity => entity.Server).NotEmpty();
        RuleFor(entity => entity.Port).NotEmpty();
        RuleFor(entity => entity.Username).NotEmpty();
        RuleFor(entity => entity.Password).NotEmpty();
        RuleFor(entity => entity.Database).NotEmpty();
    }
}
