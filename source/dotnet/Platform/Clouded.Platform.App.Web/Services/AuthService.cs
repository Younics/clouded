using Clouded.Auth.Client;
using Clouded.Auth.Shared.Token.Input;
using Clouded.Platform.App.Web.Dtos;
using Clouded.Platform.App.Web.Options;
using Clouded.Platform.App.Web.Providers;
using Clouded.Platform.App.Web.Services.Interfaces;
using Clouded.Platform.Database;
using Clouded.Platform.Database.Contexts;
using Clouded.Platform.Database.Contexts.Base;
using Clouded.Platform.Database.Entities.Clouded;
using Clouded.Shared;
using Clouded.Shared.Cryptography;
using Harbor.Library;
using Harbor.Library.Dtos;
using Harbor.Library.Enums;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using ProjectInput = Harbor.Library.Dtos.ProjectInput;

namespace Clouded.Platform.App.Web.Services;

public class AuthService(
    ApplicationOptions options,
    AuthenticationStateProvider authenticationStateProvider,
    IDbContextResolver dbContextResolver,
    IStorageService storageService
) : IAuthService
{
    private readonly TokenOptions _options = options.Clouded.Auth.Token;

    private readonly AuthClient _authClient =
        new(
            options.Clouded.Auth.ServerUrl,
            options.Clouded.Auth.ApiKey,
            options.Clouded.Auth.SecretKey
        );
    private readonly HarborClient _harborClient =
        new(
            options.Clouded.Harbor.ServerUrl,
            options.Clouded.Harbor.User,
            options.Clouded.Harbor.Password
        );

    public async Task<bool> IsAuthenticatedAsync()
    {
        var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
        return authenticationState.User.Identity?.IsAuthenticated ?? false;

        // var tokenHandler = new JwtSecurityTokenHandler();
        //
        // try
        // {
        //     var token = await _storageService.GetAccessToken();
        //
        //     if (token == null)
        //         return false;
        //
        //     tokenHandler.ValidateToken
        //     (
        //         token,
        //         new TokenValidationParameters
        //         {
        //             ValidIssuer = _options.ValidIssuer,
        //             ValidateIssuer = _options.ValidateIssuer,
        //             ValidAudiences = _options.ValidAudience,
        //             ValidateAudience = _options.ValidateAudience,
        //             ValidateIssuerSigningKey = _options.ValidateIssuerSigningKey,
        //             IssuerSigningKey = _options.SigningKey
        //         },
        //         out _
        //     );
        //
        //     return true;
        // }
        // catch
        // {
        //     return false;
        // }
    }

    // public async Task<IEnumerable<Claim>> CurrentUserClaimsAsync()
    // {
    //     try
    //     {
    //         var token = await _storageService.GetAccessToken();
    //
    //         if (token == null)
    //             return Array.Empty<Claim>();
    //
    //         return new JwtSecurityTokenHandler().ReadToken(token) is JwtSecurityToken securityToken
    //             ? securityToken.Claims
    //             : Array.Empty<Claim>();
    //     }
    //     catch (CryptographicException)
    //     {
    //         await LogoutAsync();
    //     }
    //
    //     return Array.Empty<Claim>();
    // }

    public async Task<long?> CurrentAuthIdAsync()
    {
        var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
        return long.TryParse(
            authenticationState.User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value,
            out var authId
        )
            ? authId
            : null;
    }

    public async Task<UserEntity?> CurrentUserAsync(
        DbContext? context = default,
        CancellationToken cancellationToken = default
    )
    {
        var currentUserId = await CurrentAuthIdAsync();

        if (!currentUserId.HasValue)
            return null;

        context ??= dbContextResolver.UseScopedContext<CloudedDbContext>();
        return await context.GetAsync<UserEntity>(x => x.Id == currentUserId, cancellationToken);
    }

    public async Task<bool> LoginAsync(
        string identity,
        string password,
        CancellationToken cancellationToken = default
    )
    {
        var oauth = await _authClient.Token(
            new OAuthInput { Identity = identity, Password = password },
            cancellationToken
        );

        if (oauth.Data == null)
            return false;

        await ((CloudedAuthenticationStateProvider)authenticationStateProvider).Login(
            oauth.Data.AccessToken.Token,
            oauth.Data.RefreshToken.Token
        );

        return true;
    }

    public async Task<bool> RefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        var accessToken = await storageService.GetAccessToken();
        var refreshToken = await storageService.GetRefreshToken();

        if (accessToken == null || refreshToken == null)
            return false;

        var oauth = await _authClient.TokenRefresh(accessToken, refreshToken, cancellationToken);

        if (oauth.Data == null)
            return false;

        await storageService.SaveTokens(
            oauth.Data.AccessToken.Token,
            oauth.Data.RefreshToken.Token
        );

        return true;
    }

    public async Task LogoutAsync() =>
        await ((CloudedAuthenticationStateProvider)authenticationStateProvider).Logout();

    public async Task<UserEntity?> RegisterAsync(
        RegisterInput registerInput,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    )
    {
        var created = DateTime.UtcNow;

        var result = await _authClient.Management.UserCreate(
            new Dictionary<string, object?>
            {
                { "created", created },
                { "created_ticks", created.Ticks },
                { "first_name", registerInput.FirstName },
                { "last_name", registerInput.LastName },
                { "email", registerInput.Email },
                { "password", registerInput.Password },
            },
            cancellationToken
        );

        context ??= dbContextResolver.UseScopedContext<CloudedDbContext>();
        var user = await context.GetAsync<UserEntity>((long)result.Data?["id"]!, cancellationToken);

        await RegisterHarborUserAsync(user!, context, cancellationToken);

        return user;
    }

    public async Task<UserIntegrationEntity> RegisterHarborUserAsync(
        UserEntity user,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    )
    {
        context ??= dbContextResolver.UseScopedContext<CloudedDbContext>();

        var transaction = await context.TransactionStartAsync(cancellationToken);

        var harborUserName = new CryptoAdler32().Hash($"{user.Email}_{DateTime.UtcNow.Ticks}");
        var userIntegration = new UserIntegrationEntity
        {
            UserId = user.Id,
            HarborUser = harborUserName,
            // TODO: Ensure that password (minimum of 8 characters) contains 1 Upper, 1 Number, 1 Special
            HarborPassword = Generator.RandomString(32),
            HarborProject = $"clouded-repository.{harborUserName}",
        };
        await context.CreateAsync(userIntegration, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        await _harborClient.User.Create(
            new UserInput
            {
                Name = harborUserName,
                RealName = user.FullName,
                Email = user.Email,
                Password = user.Integration.HarborPassword,
                Comment = "Clouded"
            },
            cancellationToken
        );

        await _harborClient.Project.Create(
            new ProjectInput
            {
                Name = user.Integration.HarborProject,
                IsPublic = false,
                StorageLimit = -1 // TODO: Change limit from infinite to finite
            },
            cancellationToken
        );

        await _harborClient.Project.AddMember(
            user.Integration.HarborProject,
            new ProjectMemberAddInput
            {
                Member = new MemberInput { UserName = harborUserName },
                Role = ERole.Guest
            },
            cancellationToken
        );

        await transaction.CommitAsync(cancellationToken);

        return userIntegration;
    }
}
