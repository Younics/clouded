using Clouded.Auth.Provider.DataSources.Interfaces;
using Clouded.Auth.Provider.Options;
using Microsoft.AspNetCore.Authorization;
using ApplicationOptions = Clouded.Auth.Provider.Options.ApplicationOptions;

namespace Clouded.Auth.Provider.Security.Policies;

public class BlockedAuthorizationRequirement(ApplicationOptions options)
    : AuthorizationHandler<BlockedAuthorizationRequirement>,
        IAuthorizationRequirement
{
    private readonly IdentityOptions _identityOptions = options.Clouded.Auth.Identity;

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        BlockedAuthorizationRequirement requirement
    )
    {
        var isUserIdentity = context.User.Claims.Any(x => x.Type == "user_id");
        var blocked = true;

        switch (isUserIdentity)
        {
            case true:
                blocked = Convert.ToBoolean(
                    context.User.Claims.FirstOrDefault(x => x.Type == "blocked")?.Value
                );
                break;
            case false when _identityOptions.Machine != null:
            {
                var scope = AppContext.CurrentHttpContext.RequestServices.CreateScope();
                var machineDataSource =
                    scope.ServiceProvider.GetRequiredService<IMachineDataSource>();
                var machineIdClaim = context.User.Claims.FirstOrDefault(
                    x => x.Type == "machine_id"
                );

                if (machineIdClaim != null)
                {
                    var machineId = Convert.ToInt64(machineIdClaim.Value);
                    var machine = machineDataSource.EntityFindById(machineId);

                    blocked = (bool)(
                        machine!.GetValueOrDefault(_identityOptions.Machine?.ColumnBlocked) ?? false
                    );
                }

                break;
            }
        }

        if (!blocked)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
