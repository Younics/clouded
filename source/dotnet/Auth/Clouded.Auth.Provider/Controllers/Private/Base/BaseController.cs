using System.Net.Mime;
using Clouded.Results;
using Microsoft.AspNetCore.Mvc;

namespace Clouded.Auth.Provider.Controllers.Private.Base;

[Produces(MediaTypeNames.Application.Json)]
[ExceptionCloudedResponse(StatusCodes.Status400BadRequest)]
public abstract class BaseController : ControllerBase { }
