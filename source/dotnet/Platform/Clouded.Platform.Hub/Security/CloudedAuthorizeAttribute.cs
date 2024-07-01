using Microsoft.AspNetCore.Mvc;

namespace Clouded.Platform.Hub.Security;

public class CloudedAuthorizeAttribute() : TypeFilterAttribute(typeof(CloudedAuthorizeFilter));
