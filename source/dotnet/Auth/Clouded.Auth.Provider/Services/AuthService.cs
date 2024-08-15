using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Web;
using Clouded.Auth.Provider.DataSources.Interfaces;
using Clouded.Auth.Provider.Dictionaries;
using Clouded.Auth.Provider.Dictionaries.Base;
using Clouded.Auth.Provider.Dictionaries.Enums;
using Clouded.Auth.Provider.Exceptions;
using Clouded.Auth.Provider.Options;
using Clouded.Auth.Provider.Services.Base;
using Clouded.Auth.Provider.Services.Interfaces;
using Clouded.Auth.Shared;
using Clouded.Auth.Shared.PasswordReset;
using Clouded.Auth.Shared.Token;
using Clouded.Auth.Shared.Token.Input;
using Clouded.Auth.Shared.Token.Input.Base;
using Clouded.Auth.Shared.Token.Output;
using Clouded.Core.Mail.Library.Exceptions;
using Clouded.Core.Mail.Library.Services;
using Clouded.Results.Exceptions;
using Flurl.Http;
using Microsoft.IdentityModel.Tokens;
using ApplicationOptions = Clouded.Auth.Provider.Options.ApplicationOptions;
using NotSupportedException = Clouded.Results.Exceptions.NotSupportedException;

namespace Clouded.Auth.Provider.Services;

public class AuthService(
    ApplicationOptions options,
    IHashService hashService,
    ITokenService tokenService,
    IUserDataSource userDataSource,
    IMachineDataSource machineDataSource,
    IMailService mailService
) : BaseService, IAuthService
{
    private readonly IdentityOptions _identityOptions = options.Clouded.Auth.Identity;
    private readonly MailOptions? _mailOptions = options.Clouded.Mail;
    private readonly SocialOptions _socialOptions = options.Clouded.Auth.Social;
    private readonly CloudedOptions _cloudedOptions = options.Clouded;

    /// <summary>
    /// Validate access token
    /// </summary>
    /// <param name="input"></param>
    /// <returns>Token validity data</returns>
    /// <exception cref="InvalidTokenException"></exception>
    /// <exception cref="UnauthorizedException"></exception>
    public OAuthValidateOutput Validate(OAuthAccessTokenInput input)
    {
        return tokenService.JwtTokenValidate(input);
    }

    /// <summary>
    /// Get access and refresh token
    /// </summary>
    /// <param name="input"><see cref="OAuthInput"/></param>
    /// <returns><see cref="OAuthOutput"/></returns>
    /// <exception cref="BadCredentialsException"></exception>
    /// <exception cref="UnauthorizedException"></exception>
    /// <exception cref="ColumnNotFoundException"></exception>
    public OAuthOutput Token(OAuthInput input)
    {
        var user = userDataSource.EntityFindByIdentity(input.Identity);

        if (!user.Any())
            throw new BadCredentialsException();
        
        var machine = CheckMachineKeysInput(input, user.Id);

        var hashedPassword = user[_identityOptions.User.ColumnPassword];

        if (hashedPassword == null)
            throw new ColumnNotFoundException(_identityOptions.User.ColumnPassword);

        var userId = user.Id!;
        var userSupport = userDataSource.EntitySupportTable(userId);
        var salt = (string)userSupport["salt"]!;

        if (
            (bool)(
                userSupport.GetValueOrDefault(_identityOptions.User.Support.ColumnBlocked) ?? false
            )
        )
            throw new UnauthorizedException();

        var isVerified = hashService.Verify((string)hashedPassword, input.Password, salt);
        if (!isVerified)
            throw new BadCredentialsException();

        return GetOAuthOutput(userId, machine, userSupport);
    }

    /// <summary>
    /// Remove refresh token
    /// </summary>
    /// <param name="input"></param>
    /// <exception cref="UnauthorizedException"></exception>
    /// <exception cref="InvalidTokenException"></exception>
    public void TokenRevoke(OAuthTokenRevokeInput input)
    {
        var refreshToken = userDataSource.RefreshTokenFindValid(input.RefreshToken);

        if (!refreshToken.Any())
            throw new InvalidTokenException();

        var userId = refreshToken["user_id"];

        if (userId == null)
            throw new InvalidTokenException();

        var userSupport = userDataSource.EntitySupportTable(userId);

        if (
            (bool)(
                userSupport.GetValueOrDefault(_identityOptions.User.Support.ColumnBlocked) ?? false
            )
        )
            throw new UnauthorizedException();

        if (input.AllOfUser)
        {
            userDataSource.RefreshTokensDelete(userId);
        }
        else
        {
            userDataSource.RefreshTokenDelete(input.RefreshToken);
        }
    }

    /// <summary>
    /// Refresh access and refresh token
    /// </summary>
    /// <param name="input"><see cref="OAuthTokenRefreshInput"/></param>
    /// <returns><see cref="OAuthOutput"/></returns>
    /// <exception cref="InvalidTokenException"></exception>
    /// <exception cref="UnauthorizedException"></exception>
    public OAuthOutput TokenRefresh(OAuthTokenRefreshInput input)
    {
        userDataSource.RefreshTokenDeleteExpired();

        var refreshToken = userDataSource.RefreshTokenFindValid(input.RefreshToken);
        if (!refreshToken.Any())
            throw new InvalidTokenException();

        var userId = refreshToken["user_id"];
        if (userId == null)
            throw new InvalidTokenException();

        var machine = CheckMachineKeysInput(input, userId);
        
        var userSupport = userDataSource.EntitySupportTable(userId);
        var isBlocked = (bool)(
            userSupport.GetValueOrDefault(_identityOptions.User.Support.ColumnBlocked) ?? false
        );

        if (isBlocked)
            throw new UnauthorizedException();

        userDataSource.RefreshTokenDelete(input.RefreshToken);

        return GetOAuthOutput(userId, machine, userSupport);
    }

    /// <summary>
    /// Get tokens for machine/client
    /// </summary>
    /// <param name="input"><see cref="OAuthTokenMachineInput"/></param>
    /// <returns><see cref="TokenOutput"/></returns>
    /// <exception cref="Clouded.Results.Exceptions.NotSupportedException"></exception>
    /// <exception cref="UnauthorizedException"></exception>
    public TokenOutput TokenMachine(OAuthTokenMachineInput input)
    {
        if (_identityOptions.Machine == null)
            throw new NotSupportedException("Machines are disabled.");

        var machine = machineDataSource.MachineFindByKeys(input.ApiKey, input.SecretKey);

        if (!machine.Any())
            throw new UnauthorizedException();

        if ((bool)(machine.GetValueOrDefault(_identityOptions.Machine.ColumnBlocked) ?? false))
            throw new UnauthorizedException();

        var metaData = machineDataSource.EntityMetaData(machine.Id);

        var token = tokenService.JwtTokenGenerate(
            machineId: machine.Id,
            expiresInSeconds: (double?)(machine[_identityOptions.Machine.ColumnExpiresIn]),
            blocked: machine[_identityOptions.Machine.ColumnBlocked],
            cloudedData: new { metaData }
        );

        return token;
    }

    /// <summary>
    /// Send email for reset password
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <exception cref="MailNotConfiguredException"></exception>
    /// <exception cref="BadCredentialsException"></exception>
    /// <exception cref="MissingDataException"></exception>
    /// <exception cref="UnauthorizedException"></exception>
    public async Task ForgotPasswordAsync(
        object? identity,
        CancellationToken cancellationToken = default
    )
    {
        if (_mailOptions == null)
            throw new MailNotConfiguredException();

        var user = userDataSource.EntityFindByIdentity(identity);

        if (!user.Any())
            throw new BadCredentialsException();

        var passwordResetToken = userDataSource.GeneratePasswordResetToken(user.Id!);

        var mailAddress = user.Where(
                column => _identityOptions.User.ColumnIdentityArray.Contains(column.Key)
            )
            .FirstOrDefault(
                column =>
                    column.Value != null && MailAddress.TryCreate(column.Value.ToString()!, out _)
            )
            .Value;

        if (mailAddress == null)
            throw new MissingDataException("email");

        var mailDraft = _mailOptions!.Drafts.PasswordReset;

        await mailService.SendEmailAsync(
            mailAddress.ToString()!,
            mailDraft.Subject,
            new { PasswordResetToken = passwordResetToken, mailDraft.Context },
            mailDraft.Template,
            mailDraft.From,
            cancellationToken: cancellationToken
        );
    }

    /// <summary>
    /// Reset password for identity
    /// </summary>
    /// <param name="input"><see cref="PasswordResetInput"/></param>
    /// <exception cref="BadCredentialsException"></exception>
    /// <exception cref="UnauthorizedException"></exception>
    public void ResetPassword(PasswordResetInput input)
    {
        var userSupport = userDataSource.UserSupportTableByPasswordResetToken(input.ResetToken);

        if (!userSupport.Any())
            throw new BadCredentialsException();

        var userId = userSupport["user_id"]!;

        userDataSource.EntityUpdate(
            userId,
            new UserDictionary { [_identityOptions.User.ColumnPassword] = input.Password }
        );

        userDataSource.GeneratePasswordResetToken(userId, true);
    }

    public IEnumerable<string> Permissions(OAuthAccessTokenInput input)
    {
        var validatedToken = tokenService.JwtTokenValidate(input);

        if (validatedToken.UserId == null)
        {
            //only machine permissions
            var serviceMachine = machineDataSource.EntityFindById(validatedToken.MachineId);
            return machineDataSource
                .MachinePermissionsWithRolePermissions(serviceMachine)
                .Select(p => p.Name)
                .ToList();
        }

        var machine = machineDataSource.EntityFindById(validatedToken.MachineId);
        var validPermissions = new HashSet<string>();

        var permissions = userDataSource.UserPermissionsWithRolePermissions(validatedToken.UserId);
        foreach (var permissionDictionary in permissions)
        {
            validPermissions.Add(permissionDictionary.Name);
        }

        if (!machine.Any())
        {
            return validPermissions;
        }

        var machinePermissions = machineDataSource
            .MachinePermissionsWithRolePermissions(machine)
            .Select(p => p.Name)
            .ToList();

        validPermissions.RemoveWhere(permission => !machinePermissions.Contains(permission));

        return validPermissions;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="code"></param>
    /// <exception cref="Clouded.Results.Exceptions.NotSupportedException"></exception>
    /// <exception cref="BadRequestException"></exception>
    /// <returns></returns>
    private async Task<string?> GetFacebookAccessToken(string code)
    {
        if (_socialOptions.Facebook == null)
        {
            throw new NotSupportedException();
        }

        IFlurlRequest request = new FlurlRequest(
            $"https://graph.facebook.com/v16.0/oauth/access_token?client_id={_socialOptions.Facebook.Key}&redirect_uri={GetFacebookInternalRedirectUrl()}&client_secret={_socialOptions.Facebook.Secret}&code={HttpUtility.UrlDecode(code)}"
        );

        var fbResponse = await request
            .AllowAnyHttpStatus()
            .GetJsonAsync<Dictionary<string, object>>();

        var accessToken = fbResponse.GetValueOrDefault("access_token");
        var error = fbResponse.GetValueOrDefault("error");
        if (error == null)
        {
            return accessToken?.ToString();
        }

        Console.WriteLine(error);
        throw new BadRequestException();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="code"></param>
    /// <exception cref="Clouded.Results.Exceptions.NotSupportedException"></exception>
    /// <exception cref="BadRequestException"></exception>
    /// <returns></returns>
    private async Task<string?> GetGoogleJwtToken(string code)
    {
        if (_socialOptions.Google == null)
        {
            throw new NotSupportedException();
        }

        Dictionary<string, object?> openIdConf = await GetGoogleOpenIdConf();
        openIdConf.TryGetValue("token_endpoint", out var tokenEndpoint);

        IFlurlRequest request = new FlurlRequest($"{tokenEndpoint}");

        var googleResponse = await request
            .AllowAnyHttpStatus()
            .PostJsonAsync(
                new
                {
                    code = HttpUtility.UrlDecode(code),
                    client_id = _socialOptions.Google.Key,
                    client_secret = _socialOptions.Google.Secret,
                    redirect_uri = GetGoogleInternalRedirectUrl(),
                    grant_type = "authorization_code"
                }
            )
            .ReceiveJson<Dictionary<string, object>>();

        var jwtToken = googleResponse.GetValueOrDefault("id_token");
        var error = googleResponse.GetValueOrDefault("error");
        if (error == null)
        {
            return jwtToken?.ToString();
        }

        var errorReason = googleResponse.GetValueOrDefault("error_description");
        Console.WriteLine(error);
        Console.WriteLine(errorReason);
        throw new BadRequestException();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private TokenOutput RefreshTokenGenerateAndSave(object userId)
    {
        var refreshToken = tokenService.RefreshTokenGenerate();
        userDataSource.RefreshTokenCreate(userId, refreshToken.Token, refreshToken.Expires);
        return refreshToken;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="BadRequestException"></exception>
    /// <exception cref="UnauthorizedException"></exception>
    public MachineDictionary CheckMachineKeysInput(OAuthMachineKeysInput input, object? userId)
    {
        if (input.ApiKey == null || input.SecretKey == null)
            throw new BadRequestException();

        var machine = machineDataSource.MachineFindByKeys(input.ApiKey, input.SecretKey);
        CheckEntityFound(machine, "Machine");

        var machineRoles = machineDataSource.MachineRoles(machine.Id).ToList();
        if (!machineRoles.IsNullOrEmpty() && userId != null)
        {
            var user = userDataSource.EntityFindById(userId);
            if (user.IsNullOrEmpty())
            {
                throw new BadRequestException();
            }
            
            var userRoles = userDataSource.UserRoles(user.Id).ToList();

            if (userRoles.IsNullOrEmpty())
            {
                throw new BadRequestException();
            }

            if (machineRoles.IntersectBy(userRoles.Select(x => x.Id), (y => y.Id)).IsNullOrEmpty())
            {
                // user roles and machine roles must have intersection
                throw new BadRequestException();
            }
        }

        var domains = machineDataSource.MachineDomains(machine.Id);
        AppContext.CurrentHttpContext.Request.Headers.TryGetValue("Origin", out var origin);

        switch (machine.Type)
        {
            case MachineType.Api:
            case MachineType.Service:
                break;
            case MachineType.Spa:
                var domainFound = domains.FirstOrDefault(i => (string)i.Value == origin.ToString());
                if (domainFound == null)
                {
                    throw new DomainNotAllowedException(origin.ToString());
                }

                break;
        }

        return machine;
    }

    /// <summary>
    /// </summary>
    /// <param name="input"></param>
    public OAuthOutput FacebookToken(OAuthSocialInput input)
    {
        var machine = CheckMachineKeysInput(input, input.UserId);

        userDataSource.StoreUserFacebookCode(input.Code, input.UserId);
        var userSupport = userDataSource.EntitySupportTable(input.UserId);

        return GetOAuthOutput(input.UserId, machine, userSupport);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    /// <exception cref="Clouded.Results.Exceptions.NotSupportedException"></exception>
    /// <exception cref="Clouded.Results.Exceptions.BadRequestException"></exception>
    public async Task<Dictionary<string, object?>> FacebookMe(string code)
    {
        if (_socialOptions.Facebook == null)
        {
            throw new NotSupportedException();
        }

        var accessToken = await GetFacebookAccessToken(code);

        IFlurlRequest request = new FlurlRequest(
            $"https://graph.facebook.com/me?access_token={accessToken}"
        );

        var result = await request.GetJsonAsync<Dictionary<string, object?>>();
        result.TryGetValue("id", out var userId);

        IFlurlRequest userRequest = new FlurlRequest(
            $"https://graph.facebook.com/v16.0/{userId}?access_token={accessToken}"
        );

        return await userRequest.GetJsonAsync<Dictionary<string, object?>>();
    }

    /// <summary>
    ///
    /// </summary>
    /// <exception cref="Clouded.Results.Exceptions.NotSupportedException"></exception>
    /// <returns>Facebook login URI</returns>
    public String GetFacebookRedirectUrl()
    {
        if (_socialOptions.Facebook == null)
        {
            throw new NotSupportedException();
        }

        //https://developers.facebook.com/docs/facebook-login/guides/advanced/manual-flow/
        var redirectUrl =
            $"https://www.facebook.com/v16.0/dialog/oauth?client_id={_socialOptions.Facebook.Key}&redirect_uri={GetFacebookInternalRedirectUrl()}&state={GetCsrfToken()}";

        return redirectUrl;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Clouded.Results.Exceptions.NotSupportedException"></exception>
    public async Task<string> GetGoogleRedirectUrl()
    {
        if (_socialOptions.Google == null)
        {
            throw new NotSupportedException();
        }

        Dictionary<string, object?> openIdConf = await GetGoogleOpenIdConf();
        openIdConf.TryGetValue("authorization_endpoint", out var authEndpoint);

        // https://developers.google.com/identity/openid-connect/openid-connect#server-flow
        var redirectUrl =
            $"{authEndpoint}?response_type=code&client_id={_socialOptions.Google.Key}&scope=openid%20email%20profile&redirect_uri={GetGoogleInternalRedirectUrl()}&state={GetCsrfToken()}";

        return redirectUrl;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Clouded.Results.Exceptions.NotSupportedException"></exception>
    public async Task<string> GetAppleRedirectUrl()
    {
        if (_socialOptions.Apple == null)
        {
            throw new NotSupportedException();
        }

        Dictionary<string, object?> openIdConf = await GetAppleOpenIdConf();
        openIdConf.TryGetValue("authorization_endpoint", out var authEndpoint);
        // https://www.scottbrady91.com/openid-connect/implementing-sign-in-with-apple-in-aspnet-core

        var redirectUrl =
            $"{authEndpoint}?response_type=code%20id_token&response_mode=form_post&client_id={_socialOptions.Apple.Key}&scope=openid%20email%20name&redirect_uri={GetAppleInternalRedirectUrl()}&state={GetCsrfToken()}";

        return redirectUrl;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    /// <exception cref="Clouded.Results.Exceptions.NotSupportedException"></exception>
    /// <exception cref="BadRequestException"></exception>
    public async Task<Dictionary<string, object?>> GoogleMe(string code)
    {
        if (_socialOptions.Google == null)
        {
            throw new NotSupportedException();
        }

        var jwtToken = await GetGoogleJwtToken(code);

        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(jwtToken);
        var email = jwtSecurityToken.Claims.First(i => i.Type == "email");
        var name = jwtSecurityToken.Claims.First(i => i.Type == "name");
        var givenName = jwtSecurityToken.Claims.First(i => i.Type == "given_name");
        var familyName = jwtSecurityToken.Claims.First(i => i.Type == "family_name");

        return new Dictionary<string, object?>
        {
            { "email", email.Value },
            { "name", name.Value },
            { "givenName", givenName.Value },
            { "familyName", familyName.Value },
        };
    }

    /// <summary>
    /// </summary>
    /// <param name="input"></param>
    public OAuthOutput GoogleToken(OAuthSocialInput input)
    {
        var machine = CheckMachineKeysInput(input, input.UserId);

        userDataSource.StoreUserGoogleCode(input.Code, input.UserId);
        var userSupport = userDataSource.EntitySupportTable(input.UserId);

        return GetOAuthOutput(input.UserId, machine, userSupport);
    }

    public void StoreAppleUser(string code, AppleUser userData)
    {
        userDataSource.StoreAppleData(code, userData);
    }

    /// <summary>
    /// </summary>
    /// <param name="input"></param>
    public OAuthOutput AppleToken(OAuthSocialInput input)
    {
        var machine = CheckMachineKeysInput(input, input.UserId);

        userDataSource.StoreUserAppleCode(input.Code, input.UserId);
        var userSupport = userDataSource.EntitySupportTable(input.UserId);

        return GetOAuthOutput(input.UserId, machine, userSupport);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    /// <exception cref="Clouded.Results.Exceptions.NotSupportedException"></exception>
    /// <exception cref="BadRequestException"></exception>
    public Task<Dictionary<string, object?>> AppleMe(string code)
    {
        if (_socialOptions.Apple == null)
        {
            throw new NotSupportedException();
        }

        var userAppleData = userDataSource.GetAppleData(code);

        return Task.FromResult(
            new Dictionary<string, object?>
            {
                { "email", userAppleData.GetValueOrDefault(_identityOptions.User.AppleData.ColumnEmail) },
                { "firstName", userAppleData.GetValueOrDefault(_identityOptions.User.AppleData.ColumnFirstName) },
                { "lastName", userAppleData.GetValueOrDefault(_identityOptions.User.AppleData.ColumnLastName) },
            }
        );
    }

    private OAuthOutput GetOAuthOutput(
        object userId,
        BaseDictionary machine,
        UserDictionary userSupport
    )
    {
        var metaData = userDataSource.EntityMetaData(userId);

        var jwtToken = tokenService.JwtTokenGenerate(
            userId,
            machineId: machine.Id,
            blocked: userSupport.GetValueOrDefault(_identityOptions.User.Support.ColumnBlocked),
            cloudedData: new { metaData }
        );

        return new OAuthOutput { AccessToken = jwtToken, RefreshToken = RefreshTokenGenerateAndSave(userId) };
    }

    private String GetFacebookInternalRedirectUrl()
    {
        if (_socialOptions.Facebook == null)
        {
            throw new NotSupportedException();
        }

        //https://developers.facebook.com/docs/facebook-login/guides/advanced/manual-flow/
        return $"{_cloudedOptions.Domain}/v1/{RoutesConfig.SocialRoutePrefix}/facebook/login/backlink";
    }

    private String GetGoogleInternalRedirectUrl()
    {
        if (_socialOptions.Google == null)
        {
            throw new NotSupportedException();
        }

        return $"{_cloudedOptions.Domain}/v1/{RoutesConfig.SocialRoutePrefix}/google/login/backlink";
    }

    private String GetAppleInternalRedirectUrl()
    {
        if (_socialOptions.Apple == null)
        {
            throw new NotSupportedException();
        }

        return $"{_cloudedOptions.Domain}/v1/{RoutesConfig.SocialRoutePrefix}/apple/login/backlink";
    }

    private static async Task<Dictionary<string, object?>> GetGoogleOpenIdConf()
    {
        IFlurlRequest request = new FlurlRequest(
            "https://accounts.google.com/.well-known/openid-configuration"
        );

        var openIdConf = await request.GetJsonAsync<Dictionary<string, object?>>();
        return openIdConf;
    }

    private static async Task<Dictionary<string, object?>> GetAppleOpenIdConf()
    {
        IFlurlRequest request = new FlurlRequest(
            "https://appleid.apple.com/.well-known/openid-configuration"
        );

        var openIdConf = await request.GetJsonAsync<Dictionary<string, object?>>();
        return openIdConf;
    }

    private static string GetCsrfToken()
    {
        var guid = Guid.NewGuid();

        return BitConverter.ToString(guid.ToByteArray());
    }
}
