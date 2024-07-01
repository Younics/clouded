using System.Net;
using Flurl;
using Flurl.Http;
using WebSupport.Library.Dtos;

namespace WebSupport.Library;

public class WebSupportUserClient(string apiUrl, string secret) : BaseClient(apiUrl, secret)
{
    private const string UserSelfUrlSegment = "/v1/user/self";

    public async Task<UserOutput?> Me(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await ApiUri
                .AppendPathSegment(UserSelfUrlSegment)
                .WithHeaders(MakeHeaders(HttpMethod.Get, UserSelfUrlSegment))
                .GetJsonAsync<UserOutput>(
                    HttpCompletionOption.ResponseContentRead,
                    cancellationToken
                );

            return response;
        }
        catch (FlurlHttpException ex)
        {
            if (ex.StatusCode == (int)HttpStatusCode.NotFound)
                return null;

            throw;
        }
    }
}
