using Microsoft.AspNetCore.Mvc;

namespace Clouded.Function.Api.Security;

public class CloudedAuthorizeAttribute() : TypeFilterAttribute(typeof(CloudedAuthorizationFilter));
