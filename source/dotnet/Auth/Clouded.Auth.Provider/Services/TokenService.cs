using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Clouded.Auth.Provider.DataSources.Interfaces;
using Clouded.Auth.Provider.Dictionaries;
using Clouded.Auth.Provider.Dictionaries.Enums;
using Clouded.Auth.Provider.Exceptions;
using Clouded.Auth.Provider.Options;
using Clouded.Auth.Provider.Services.Interfaces;
using Clouded.Auth.Shared.Token.Input;
using Clouded.Auth.Shared.Token.Output;
using Clouded.Shared;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using static System.Int64;
using ApplicationOptions = Clouded.Auth.Provider.Options.ApplicationOptions;

namespace Clouded.Auth.Provider.Services;

public class TokenService : ITokenService
{
    private readonly IUserDataSource _userDataSource;
    private readonly IMachineDataSource _machineDataSource;
    private readonly AuthOptions _authOptions;
    private readonly List<string> _validAudiences;
    private const string UserClaimType = "user_id";
    private const string MachineClaimType = "machine_id";

    public TokenService(
        ApplicationOptions options,
        IUserDataSource userDataSource,
        IDomainDataSource domainDataSource,
        IMachineDataSource machineDataSource
    )
    {
        _userDataSource = userDataSource;
        _machineDataSource = machineDataSource;
        _authOptions = options.Clouded.Auth;
        _validAudiences = domainDataSource.Entities().Select(i => (i.Value as string)!).ToList();
    }

    public OAuthValidateOutput JwtTokenValidate(OAuthAccessTokenInput input)
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = _authOptions.Token.ValidIssuer,
            ValidateIssuer = _authOptions.Token.ValidateIssuer,
            ValidAudiences = _validAudiences,
            ValidateAudience = _authOptions.Token.ValidateAudience,
            ValidateIssuerSigningKey = _authOptions.Token.ValidateIssuerSigningKey,
            IssuerSigningKey = _authOptions.Token.SigningKey,
            ValidateLifetime = true
        };

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(input.AccessToken);

            var identities = Array.Empty<object?>();
            object? machineId = null;

            if (_authOptions.Identity.Machine != null)
            {
                machineId = token.Claims.FirstOrDefault(i => i.Type == MachineClaimType)?.Value;
                var machine = _machineDataSource.EntityFindById(machineId);
                if (machine.Any())
                {
                    if (machine.Type == MachineType.Service)
                        validationParameters.ValidateAudience = false;
                    
                    identities =
                    [
                        machine.GetValueOrDefault(
                            _authOptions.Identity.Machine.ColumnIdentity,
                            string.Empty
                        )
                    ];
                }
            }
            
            var principal = handler.ValidateToken(input.AccessToken, validationParameters, out _);
            if (principal == null)
                throw new InvalidTokenException();

            var userId = principal.Claims.FirstOrDefault(i => i.Type == UserClaimType)?.Value;
            var user = _userDataSource.EntityFindById(userId);
            if (user.Any())
            {
                identities = user
                    .Where(
                        x =>
                            _authOptions.Identity.User.ColumnIdentityArray.Contains(x.Key)
                            && x.Value != null
                    )
                    .Select(x => x.Value)
                    .ToArray();
            }

            TryParse
            (
                principal.Claims.First(i => i.Type == "exp").Value.ToString(),
                out var expiresAt
            );
            
            return new OAuthValidateOutput
            {
                Identities = identities!,
                UserId = userId,
                MachineId = machineId,
                ExpiresAt = DateTimeOffset.FromUnixTimeSeconds(expiresAt).UtcDateTime
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new InvalidTokenException(e);
        }
    }

    public TokenOutput JwtTokenGenerate(
        object? userId = null,
        object? machineId = null,
        double? expiresInSeconds = null,
        object? blocked = null,
        object? cloudedData = null
    )
    {
        cloudedData ??= new { };

        var claims = new List<Claim>
        {
            new("clouded", JsonConvert.SerializeObject(cloudedData), JsonClaimValueTypes.Json),
            new("blocked", $"{blocked ?? false}")
        };

        if (userId != null)
            claims.Add(new Claim(UserClaimType, userId.ToString() ?? string.Empty));

        if (machineId != null)
            claims.Add(new Claim("machine_id", machineId.ToString() ?? string.Empty));

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _authOptions.Token.ValidIssuer,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddSeconds(
                expiresInSeconds ?? _authOptions.Token.AccessTokenExpiration
            ),
            SigningCredentials = new SigningCredentials(
                _authOptions.Token.SigningKey,
                SecurityAlgorithms.HmacSha512Signature
            ),
            Claims = new Dictionary<string, object>
            {
                { JwtRegisteredClaimNames.Aud, _validAudiences }
            }
        };

        var jwtToken = tokenHandler.CreateToken(tokenDescriptor);
        return new TokenOutput
        {
            Token = tokenHandler.WriteToken(jwtToken),
            Expires = tokenDescriptor.Expires.Value
        };
    }

    public TokenOutput RefreshTokenGenerate()
    {
        var token =
            $"{Generator.RandomString(48, includeSpecialCharacters: false)}"
            + $"{DateTime.UtcNow.Ticks}"
            + $"{Generator.RandomString(48, includeSpecialCharacters: false)}";

        return new TokenOutput
        {
            Token = token,
            Expires = DateTime.UtcNow.AddSeconds(_authOptions.Token.RefreshTokenExpiration)
        };
    }
}
