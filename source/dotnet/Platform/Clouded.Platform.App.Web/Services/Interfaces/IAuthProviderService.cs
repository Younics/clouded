using Clouded.Platform.App.Web.Dtos;
using Clouded.Platform.Database.Entities.Clouded;

namespace Clouded.Platform.App.Web.Services.Interfaces;

public interface IAuthProviderService : IProviderService<AuthProviderEntity, AuthProviderInput>
{
    public void PrepareInput(AuthProviderEntity entity, AuthProviderInput input);
}
