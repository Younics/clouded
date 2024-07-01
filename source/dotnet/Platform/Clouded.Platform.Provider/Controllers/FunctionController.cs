using Clouded.Platform.Models.Dtos.Provider;
using Clouded.Platform.Provider.Security;
using Clouded.Platform.Provider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Clouded.Platform.Provider.Controllers;

[ApiController]
[Route("v1/function")]
public class FunctionController(IProviderService providerService) : ControllerBase
{
    [CloudedAuthorize]
    [HttpPost("build")]
    public async Task<IActionResult> Build([FromBody] FunctionBuildInput input)
    {
        await providerService.FunctionBuildAndPush(input);
        return Ok();
    }
}
