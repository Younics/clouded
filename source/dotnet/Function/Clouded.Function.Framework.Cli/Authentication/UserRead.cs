using Clouded.Function.Framework.Attributes;
using Clouded.Function.Framework.Components;
using Clouded.Function.Framework.Contexts;
using Clouded.Function.Framework.Outputs;

namespace Clouded.Function.Framework.Cli.Authentication;

[CloudedMap("UserReadValidationCustom")]
public class UserRead : ValidationHook
{
    public override Task<ValidationOutput> RunAsync(ValidationContext context)
    {
        return Task.FromResult(
            new ValidationOutput
            {
                Code = 112133,
                I18N = "validation.user-read.forbidden",
                Message = "User read is forbidden",
                Passed = false,
            }
        );
    }
}
