using Clouded.Platform.App.Web.Dtos;
using Clouded.Platform.Models.Enums;
using FluentValidation;

namespace Clouded.Platform.App.Web.Validators;

public class AuthProviderValidator : AbstractValidator<AuthProviderInput>
{
    public AuthProviderValidator()
    {
        RuleFor(entity => entity.Name).NotEmpty();
        RuleFor(entity => entity.Code).MaximumLength(60).NotEmpty();
        RuleFor(entity => entity.DomainId).NotEmpty();
        RuleFor(entity => entity.DataSourceProviderId).NotEmpty();

        RuleFor(x => x.Configuration.Hash.AlgorithmType).NotEmpty().IsInEnum();
        RuleFor(x => x.Configuration.Hash.Secret).NotEmpty();

        RuleFor(x => x.Configuration.Cors.AllowedOrigins).NotEmpty();
        RuleFor(x => x.Configuration.Cors.AllowedMethods).NotEmpty();
        When(
            x => x.Configuration.Cors.SupportsCredentials,
            () =>
            {
                RuleFor(x => x.Configuration.Cors.AllowedOrigins)
                    .Must(val => val.Count > 1 || (val.Count == 1 && val[0] != "*"))
                    .WithMessage(
                        "Allowed origins cannot be [*] when Supports credentials is enabled"
                    );
            }
        );

        When(
            customer => customer.Configuration.Hash.AlgorithmType == EHashType.Argon2,
            () =>
            {
                RuleFor(x => x.Configuration.ArgonInput.Iterations).NotEmpty();
                RuleFor(x => x.Configuration.ArgonInput.MemorySize).NotEmpty();
                RuleFor(x => x.Configuration.ArgonInput.ReturnBytes).NotEmpty();
                RuleFor(x => x.Configuration.ArgonInput.DegreeOfParallelism).NotEmpty();
            }
        );

        RuleFor(x => x.Configuration.Token.ValidIssuer).NotEmpty();
        RuleFor(x => x.Configuration.Token.AccessTokenExpiration).NotEmpty();
        RuleFor(x => x.Configuration.Token.RefreshTokenExpiration).NotEmpty();

        RuleForEach(entity => entity.Configuration.Management.Users)
            .ChildRules(x =>
            {
                x.RuleFor(y => y.Identity).NotEmpty();
                x.RuleFor(y => y.Password).NotEmpty();
            });

        When(
            customer => customer.Configuration.Mail.Enabled,
            () =>
            {
                RuleFor(x => x.Configuration.Mail.Host).NotEmpty();
                RuleFor(x => x.Configuration.Mail.From).NotEmpty().EmailAddress();
                RuleFor(x => x.Configuration.Mail.Port).NotEmpty();
                RuleFor(x => x.Configuration.Mail.ResetPasswordReturnUrl)
                    .NotEmpty()
                    .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                    .WithMessage("{PropertyName} must be valid URL address.");

                When(
                    customer => customer.Configuration.Mail.Authentication,
                    () =>
                    {
                        RuleFor(x => x.Configuration.Mail.User).NotEmpty();
                        RuleFor(x => x.Configuration.Mail.Password).NotEmpty();
                    }
                );
            }
        );

        When(
            customer => customer.Configuration.Google.Enabled,
            () =>
            {
                RuleFor(x => x.Configuration.Google)
                    .SetValidator(new AuthProviderSocialValidator());
            }
        );

        When(
            customer => customer.Configuration.Facebook.Enabled,
            () =>
            {
                RuleFor(x => x.Configuration.Facebook)
                    .SetValidator(new AuthProviderSocialValidator());
            }
        );

        When(
            customer => customer.Configuration.Apple.Enabled,
            () =>
            {
                RuleFor(x => x.Configuration.Apple).SetValidator(new AuthProviderSocialValidator());
            }
        );

        RuleFor(x => x.Configuration.IdentityUser)
            .SetValidator(new AuthProviderIdentityUserValidator());

        When(
            customer => customer.Configuration.IdentityOrganization.Enabled,
            () =>
            {
                RuleFor(x => x.Configuration.IdentityOrganization)
                    .SetValidator(new AuthProviderIdentityValidator());
            }
        );

        When(
            customer => customer.Configuration.IdentityRole.Enabled,
            () =>
            {
                RuleFor(x => x.Configuration.IdentityRole)
                    .SetValidator(new AuthProviderIdentityValidator());
            }
        );

        When(
            customer => customer.Configuration.IdentityPermission.Enabled,
            () =>
            {
                RuleFor(x => x.Configuration.IdentityPermission)
                    .SetValidator(new AuthProviderIdentityValidator());
            }
        );
    }

    class AuthProviderIdentityValidator : AbstractValidator<AuthProviderIdentityInput>
    {
        public AuthProviderIdentityValidator()
        {
            RuleFor(x => x.Schema).NotEmpty();
            RuleFor(x => x.Table).NotEmpty();
            RuleFor(x => x.ColumnIdentity).NotEmpty();
            RuleFor(x => x.ColumnId).NotEmpty();
        }
    }

    class AuthProviderIdentityUserValidator : AbstractValidator<AuthProviderIdentityUserInput>
    {
        public AuthProviderIdentityUserValidator()
        {
            RuleFor(x => x.Schema).NotEmpty();
            RuleFor(x => x.Table).NotEmpty();
            RuleFor(x => x.ColumnIdentity).NotEmpty();
            RuleFor(x => x.ColumnId).NotEmpty();
            RuleFor(x => x.ColumnPassword).NotEmpty();
        }
    }

    class AuthProviderSocialValidator : AbstractValidator<AuthProviderSocialInput>
    {
        public AuthProviderSocialValidator()
        {
            RuleFor(x => x.Key).NotEmpty();
            RuleFor(x => x.Secret).NotEmpty();
            RuleFor(x => x.RedirectUrl)
                .NotEmpty()
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage("{PropertyName} must be valid URL address.");
            RuleFor(x => x.DeniedRedirectUrl)
                .NotEmpty()
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage("{PropertyName} must be valid URL address.");
        }
    }
}
