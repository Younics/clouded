using Clouded.Function.Api.Options;
using Clouded.Function.Api.Security;
using Clouded.Function.Library.Dtos;
using Clouded.Function.Library.Services;
using Clouded.Function.Shared;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Clouded.Function.Api.Controllers;

[ApiController]
[Route("v1/execute")]
public class ExecutionController(ApplicationOptions options, IFunctionService functionService)
    : ControllerBase
{
    [CloudedAuthorize]
    [HttpPost("function")]
    public async Task<IActionResult> Execute([FromBody] FunctionRequest request)
    {
        ExecutionResponse response;

        try
        {
            response = await functionService.Execute(
                options.Clouded.Function.Hooks.First(i => i.Key == request.Name).Value,
                request.Name,
                EFunctionType.Function,
                request.Context
            );
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Function Error");

            return BadRequest();
        }

        return Ok(response);
    }

    [CloudedAuthorize]
    [HttpPost("hook")]
    public async Task<IActionResult> HookExecute([FromBody] HookRequest request)
    {
        try
        {
            var response = await functionService.Execute(
                options.Clouded.Function.Hooks.First(i => i.Key == request.Name).Value,
                request.Name,
                request.Type,
                request.Context
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Function Error");

            return BadRequest();
        }
    }
}
