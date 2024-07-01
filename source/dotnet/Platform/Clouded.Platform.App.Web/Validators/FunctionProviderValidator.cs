using Clouded.Platform.App.Web.Dtos;
using FluentValidation;

namespace Clouded.Platform.App.Web.Validators;

public class FunctionProviderValidator : AbstractValidator<FunctionProviderInput>
{
    public FunctionProviderValidator()
    {
        RuleFor(entity => entity.Name).NotEmpty();
        RuleFor(entity => entity.RepositoryId).NotEmpty();
    }
}
