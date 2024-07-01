using Microsoft.AspNetCore.Mvc;

namespace Clouded.Auth.Provider.Security;

public class CloudedAuthorizeAttribute() : TypeFilterAttribute(typeof(CloudedAuthorizationFilter));
