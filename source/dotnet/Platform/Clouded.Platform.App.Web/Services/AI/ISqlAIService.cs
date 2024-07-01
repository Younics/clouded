using Clouded.Core.DataSource.Shared;
using Clouded.Platform.App.Web.Services.Interfaces;

namespace Clouded.Platform.App.Web.Services.AI;

public interface ISqlAIService : IBaseService
{
    public Task<string> Generate(
        DatabaseType type,
        string schema,
        string input,
        CancellationToken cancellationToken = default
    );
}
