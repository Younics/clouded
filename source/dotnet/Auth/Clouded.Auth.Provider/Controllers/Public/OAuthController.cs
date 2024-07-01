using System.Net.Mime;
using Clouded.Auth.Provider.Services.Interfaces;
using Clouded.Auth.Shared;
using Clouded.Auth.Shared.PasswordReset;
using Clouded.Auth.Shared.Token.Input;
using Clouded.Auth.Shared.Token.Output;
using Clouded.Results;
using Microsoft.AspNetCore.Mvc;

namespace Clouded.Auth.Provider.Controllers.Public;

[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
[ExceptionCloudedResponse(StatusCodes.Status400BadRequest)]
[ApiController]
[Route($"v1/{RoutesConfig.OAuthRoutePrefix}")]
public class OAuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Validate access token
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("token/validate")]
    [SuccessCloudedResponse<OAuthValidateOutput>]
    public IActionResult Validate([FromBody] OAuthAccessTokenInput input)
    {
        return Ok(authService.Validate(input));
    }

    /// <summary>
    /// Authenticate user and return access/refresh token
    /// </summary>
    /// <param name="input"><see cref="OAuthInput"/></param>
    /// <returns>New set of tokens</returns>
    [HttpPost("token")]
    [SuccessCloudedResponse<OAuthOutput>]
    [ExceptionCloudedResponse(StatusCodes.Status401Unauthorized)]
    [ExceptionCloudedResponse(StatusCodes.Status400BadRequest)]
    public IActionResult TokenAccess([FromBody] OAuthInput input)
    {
        return Ok(authService.Token(input));
    }

    /// <summary>
    /// Revoke the refresh token.
    /// </summary>
    /// <param name="input"><see cref="OAuthTokenRevokeInput"/></param>
    /// <returns></returns>
    [HttpPost("token/revoke")]
    [SuccessCloudedResponse]
    [ExceptionCloudedResponse(StatusCodes.Status401Unauthorized)]
    public IActionResult TokenRevoke([FromBody] OAuthTokenRevokeInput input)
    {
        authService.TokenRevoke(input);
        return Ok();
    }

    /// <summary>
    /// Refresh tokens.
    /// </summary>
    /// <param name="input"><see cref="OAuthTokenRefreshInput"/></param>
    /// <returns>New set of tokens</returns>
    [HttpPost("token/refresh")]
    [SuccessCloudedResponse<OAuthOutput>]
    [ExceptionCloudedResponse(StatusCodes.Status401Unauthorized)]
    public IActionResult TokenRefresh([FromBody] OAuthTokenRefreshInput input)
    {
        return Ok(authService.TokenRefresh(input));
    }

    /// <summary>
    /// Get authentication token for machine/client
    /// </summary>
    /// <param name="input"><see cref="OAuthTokenMachineInput"/></param>
    /// <returns></returns>
    [HttpPost("token/machine")]
    [SuccessCloudedResponse<TokenOutput>]
    [ExceptionCloudedResponse(StatusCodes.Status401Unauthorized)]
    [ExceptionCloudedResponse(StatusCodes.Status501NotImplemented)]
    public IActionResult TokenMachine([FromBody] OAuthTokenMachineInput input)
    {
        return Ok(authService.TokenMachine(input));
    }

    /// <summary>
    /// Forgot password request
    /// </summary>
    /// <param name="input"><see cref="ForgotPasswordInput"/></param>
    /// <returns></returns>
    [HttpPost("forgot-password")]
    [SuccessCloudedResponse]
    [ExceptionCloudedResponse(StatusCodes.Status400BadRequest)]
    [ExceptionCloudedResponse(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordInput input)
    {
        await authService.ForgotPasswordAsync(input.Identity);
        return Ok();
    }

    /// <summary>
    /// Reset password
    /// </summary>
    /// <param name="input"><see cref="PasswordResetInput"/></param>
    /// <returns></returns>
    [HttpPost("reset-password")]
    [SuccessCloudedResponse]
    [ExceptionCloudedResponse(StatusCodes.Status400BadRequest)]
    public IActionResult ResetPassword([FromBody] PasswordResetInput input)
    {
        authService.ResetPassword(input);
        return Ok();
    }

    /// <summary>
    /// Get permissions
    /// </summary>
    /// <param name="input"><see cref="OAuthAccessTokenInput"/></param>
    /// <returns>Set of permissions for token</returns>
    [HttpPost("permissions")]
    [SuccessCloudedResponse<IEnumerable<string>>]
    [ExceptionCloudedResponse(StatusCodes.Status404NotFound)]
    public IActionResult Permissions([FromBody] OAuthAccessTokenInput input)
    {
        return Ok(authService.Permissions(input));
    }
}
