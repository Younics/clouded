using Clouded.Platform.App.Web.Dtos;
using Clouded.Platform.Database.Entities.Clouded;
using Microsoft.EntityFrameworkCore;

namespace Clouded.Platform.App.Web.Services.Interfaces;

public interface IProjectService : IBaseService
{
    public Task<ProjectEntity?> Create(
        ProjectInput projectInput,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    );
}
