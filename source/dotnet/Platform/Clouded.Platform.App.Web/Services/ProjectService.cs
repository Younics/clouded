using Clouded.Platform.App.Web.Services.Interfaces;
using Clouded.Platform.Database;
using Clouded.Platform.Database.Contexts;
using Clouded.Platform.Database.Contexts.Base;
using Clouded.Platform.Database.Entities.Clouded;
using Clouded.Shared;
using Microsoft.EntityFrameworkCore;
using ProjectInput = Clouded.Platform.App.Web.Dtos.ProjectInput;

namespace Clouded.Platform.App.Web.Services;

public class ProjectService(IDbContextResolver dbContextResolver, IAuthService authService)
    : IProjectService
{
    public async Task<ProjectEntity?> Create(
        ProjectInput projectInput,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    )
    {
        context ??= dbContextResolver.UseScopedContext<CloudedDbContext>();

        var currentUser = await authService.CurrentUserAsync(cancellationToken: cancellationToken);
        if (currentUser == null)
            return null;

        var transaction = await context.TransactionStartAsync(cancellationToken);
        var project = new ProjectEntity
        {
            Name = projectInput.Name!,
            Code =
                projectInput.Name!.ToSnakeCase()
                + $"-{DateTime.UtcNow.ToString("O").GetHashCode():X}".ToLower(),
            Description = projectInput.Description,
            RegionId = projectInput.RegionId!.Value,
            UsersRelation = new[] { new ProjectUserRelationEntity { UserId = currentUser.Id } }
        };

        await context.CreateAsync(project, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return project;
    }
}
