using Clouded.Platform.Models.Dtos.Provider;
using Clouded.Platform.Provider.Security;
using Clouded.Platform.Provider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Clouded.Platform.Provider.Controllers;

[ApiController]
[Route("v1/provider")]
public class ProviderController(IProviderService providerService) : ControllerBase
{
    [CloudedAuthorize]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] ProviderCreateInput input)
    {
        await providerService.CreateAsync(input, HttpContext.RequestAborted);
        return Ok();
    }

    [CloudedAuthorize]
    [HttpPatch("{name}/start")]
    public async Task<IActionResult> Start(
        [FromRoute] string name,
        [FromBody] Dictionary<string, string>? envs
    )
    {
        await providerService.StartAsync(name, envs, HttpContext.RequestAborted);
        return Ok();
    }

    [CloudedAuthorize]
    [HttpPatch("{name}/stop")]
    public async Task<IActionResult> Stop([FromRoute] string name)
    {
        await providerService.StopAsync(name, HttpContext.RequestAborted);
        return Ok();
    }

    [CloudedAuthorize]
    [HttpDelete("{name}/delete")]
    public async Task<IActionResult> Delete([FromRoute] string name)
    {
        await providerService.DeleteAsync(name, HttpContext.RequestAborted);
        return Ok();
    }
}
