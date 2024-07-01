using Clouded.Platform.App.Web.Dtos;
using FluentValidation;

namespace Clouded.Platform.App.Web.Validators;

public class DomainValidator : AbstractValidator<DomainInput>
{
    public DomainValidator()
    {
        RuleFor(entity => entity.Value)
            .NotEmpty()
            .WithName("Address")
            .Matches(@"^((?!-)[A-Za-z0-9-]{1,63}(?<!-)\.)+[A-Za-z]{2,6}$")
            .WithMessage("Address must be valid domain address.");
    }
}
