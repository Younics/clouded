using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Clouded.Auth.Provider.Options;
using Clouded.Auth.Provider.Services.Interfaces;
using Clouded.Auth.Shared;
using Clouded.Auth.Shared.Token.Input;
using Clouded.Auth.Shared.Token.Output;
using Clouded.Results;
using Microsoft.AspNetCore.Mvc;
using ApplicationOptions = Clouded.Auth.Provider.Options.ApplicationOptions;
using NotSupportedException = Clouded.Results.Exceptions.NotSupportedException;

namespace Clouded.Auth.Provider.Controllers.Public;

[ApiController]
[Route($"v1/{RoutesConfig.SocialRoutePrefix}")]
public class SocialController(IAuthService authService, ApplicationOptions options) : ControllerBase
{
    private readonly SocialOptions _socialOptions = options.Clouded.Auth.Social;

    /// <summary>
    /// Start Facebook login
    /// </summary>
    /// <exception cref="Clouded.Results.Exceptions.NotSupportedException"></exception>
    /// <returns></returns>
    [HttpGet("facebook/login-url")]
    [SuccessCloudedResponse<string>]
    [ExceptionCloudedResponse(StatusCodes.Status501NotImplemented)]
    public IActionResult FacebookLoginUrl()
    {
        return Ok(authService.GetFacebookRedirectUrl());
    }

    /// <summary>
    /// Get Facebook user profile data
    /// </summary>
    /// <exception cref="Clouded.Results.Exceptions.NotSupportedException"></exception>
    /// <returns></returns>
    [HttpGet("facebook/me/{code}")]
    [SuccessCloudedResponse<Dictionary<string, object>>]
    [ExceptionCloudedResponse(StatusCodes.Status501NotImplemented)]
    [ExceptionCloudedResponse(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> FacebookMe([Required] [FromRoute] string code)
    {
        return Ok(await authService.FacebookMe(code));
    }

    /// <summary>
    /// Get token by Facebook login
    /// </summary>
    /// <exception cref="Clouded.Results.Exceptions.NotSupportedException"></exception>
    /// <returns></returns>
    [HttpPost("facebook/token")]
    [SuccessCloudedResponse<OAuthOutput>]
    [ExceptionCloudedResponse(StatusCodes.Status501NotImplemented)]
    public IActionResult FacebookToken([Required] [FromBody] OAuthSocialInput input) =>
        Ok(authService.FacebookToken(input));

    /// <summary>
    ///
    /// </summary>
    /// <exception cref="Clouded.Results.Exceptions.NotSupportedException"></exception>
    /// <returns></returns>
    [HttpGet("facebook/login/backlink")]
    [ExceptionCloudedResponse(StatusCodes.Status501NotImplemented)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult FacebookLoginBacklink(
        [Optional] [FromQuery] string? error,
        [Optional] [FromQuery] string? code,
        [Optional] [FromQuery] string? state
    )
    {
        if (_socialOptions.Facebook == null)
        {
            throw new NotSupportedException();
        }

        //todo validate state

        if (error != null || code == null)
        {
            var deniedUrl = _socialOptions.Facebook.DeniedRedirectUrl;
            if (!string.IsNullOrEmpty(state))
            {
                deniedUrl += deniedUrl.Contains('?') ? $"&state={state}" : $"?state={state}";
            }
            return RedirectPreserveMethod(deniedUrl);
        }

        var redirectUrl = $"{_socialOptions.Facebook.RedirectUrl}?access_code={code}";
        if (!string.IsNullOrEmpty(state))
        {
            redirectUrl += $"&state={state}";
        }
        return RedirectPreserveMethod(redirectUrl);
    }

    /// <summary>
    /// Start Google login
    /// </summary>
    /// <exception cref="Clouded.Results.Exceptions.NotSupportedException"></exception>
    /// <returns></returns>
    [HttpGet("google/login-url")]
    [SuccessCloudedResponse<string>]
    [ExceptionCloudedResponse(StatusCodes.Status501NotImplemented)]
    public async Task<IActionResult> GoogleLoginUrl()
    {
        return Ok(await authService.GetGoogleRedirectUrl());
    }

    /// <summary>
    /// Get Google user profile data
    /// </summary>
    /// <exception cref="Clouded.Results.Exceptions.NotSupportedException"></exception>
    /// <returns></returns>
    [HttpGet("google/me/{code}")]
    [SuccessCloudedResponse<Dictionary<string, object>>]
    [ExceptionCloudedResponse(StatusCodes.Status501NotImplemented)]
    [ExceptionCloudedResponse(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GoogleMe([Required] [FromRoute] string code)
    {
        return Ok(await authService.GoogleMe(code));
    }

    /// <summary>
    /// Get token by Google login
    /// </summary>
    /// <exception cref="Clouded.Results.Exceptions.NotSupportedException"></exception>
    /// <returns></returns>
    [HttpPost("google/token")]
    [SuccessCloudedResponse<OAuthOutput>]
    [ExceptionCloudedResponse(StatusCodes.Status501NotImplemented)]
    public IActionResult GoogleToken([Required] [FromBody] OAuthSocialInput input) =>
        Ok(authService.GoogleToken(input));

    /// <summary>
    ///
    /// </summary>
    /// <exception cref="Clouded.Results.Exceptions.NotSupportedException"></exception>
    /// <returns></returns>
    [HttpGet("google/login/backlink")]
    [ExceptionCloudedResponse(StatusCodes.Status501NotImplemented)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult GoogleLoginBacklink(
        [Optional] [FromQuery] string? error,
        [Optional] [FromQuery] string? code,
        [Optional] [FromQuery] string? state
    )
    {
        if (_socialOptions.Google == null)
        {
            throw new NotSupportedException();
        }

        //todo validate state

        if (error != null || code == null)
        {
            var deniedUrl = _socialOptions.Google.DeniedRedirectUrl;
            if (!string.IsNullOrEmpty(state))
            {
                deniedUrl += deniedUrl.Contains('?') ? $"&state={state}" : $"?state={state}";
            }
            return RedirectPreserveMethod(deniedUrl);
        }

        var redirectUrl = $"{_socialOptions.Google.RedirectUrl}?access_code={code}";
        if (!string.IsNullOrEmpty(state))
        {
            redirectUrl += $"&state={state}";
        }
        return RedirectPreserveMethod(redirectUrl);
    }

    /// <summary>
    /// Start Apple login
    /// </summary>
    /// <exception cref="Clouded.Results.Exceptions.NotSupportedException"></exception>
    /// <returns></returns>
    [HttpGet("apple/login-url")]
    [SuccessCloudedResponse<string>]
    [ExceptionCloudedResponse(StatusCodes.Status501NotImplemented)]
    public async Task<IActionResult> AppleLoginUrl()
    {
        return Ok(await authService.GetAppleRedirectUrl());
    }

    /// <summary>
    /// Get Apple user profile data
    /// </summary>
    /// <exception cref="Clouded.Results.Exceptions.NotSupportedException"></exception>
    /// <returns></returns>
    [HttpGet("apple/me/{code}")]
    [SuccessCloudedResponse<Dictionary<string, object>>]
    [ExceptionCloudedResponse(StatusCodes.Status501NotImplemented)]
    [ExceptionCloudedResponse(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AppleMe([Required] [FromRoute] string code)
    {
        return Ok(await authService.AppleMe(code));
    }

    /// <summary>
    /// Get token by Apple login
    /// </summary>
    /// <exception cref="Clouded.Results.Exceptions.NotSupportedException"></exception>
    /// <returns></returns>
    [HttpPost("apple/token")]
    [SuccessCloudedResponse<OAuthOutput>]
    [ExceptionCloudedResponse(StatusCodes.Status501NotImplemented)]
    public IActionResult AppleToken([Required] [FromBody] OAuthSocialInput input)
    {
        return Ok(authService.AppleToken(input));
    }

    /// <summary>
    ///
    /// </summary>
    /// <exception cref="Clouded.Results.Exceptions.NotSupportedException"></exception>
    /// <returns></returns>
    [HttpPost("apple/login/backlink")]
    [ExceptionCloudedResponse(StatusCodes.Status501NotImplemented)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult AppleLoginBacklink([FromBody] AppleLoginResponse response)
    {
        if (_socialOptions.Apple == null)
        {
            throw new NotSupportedException();
        }

        //todo validate state

        if (response.Code == null)
        {
            return RedirectPreserveMethod(_socialOptions.Apple.DeniedRedirectUrl);
        }

        if (response.User != null)
        {
            authService.StoreAppleUser(response.Code, response.User);
        }

        return RedirectPreserveMethod(
            $"{_socialOptions.Apple.RedirectUrl}?access_code={response.Code}"
        );
    }
}
