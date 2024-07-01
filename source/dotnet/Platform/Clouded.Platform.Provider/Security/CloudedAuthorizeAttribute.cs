using Microsoft.AspNetCore.Mvc;

namespace Clouded.Platform.Provider.Security;

public class CloudedAuthorizeAttribute() : TypeFilterAttribute(typeof(CloudedAuthorizationFilter));
