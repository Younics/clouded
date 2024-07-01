// See https://aka.ms/new-console-template for more information

using Clouded.Auth.Client;
using Clouded.Auth.Shared.Token.Input;

var auth = new AuthClient(
    "http://localhost:8001",
    "R25oLWmBiHSeLzkMgCor1rUVfrpfECt6638506034153438880anRUFPylkhnV828MzZlPHu1dgtFHm2v2",
    "NrPx5XhxKa3WY4AZzazutOXEaFxSnB5TA2Eo2FiQmdelS0U8"
);

// var userCreate = await auth.Management.UserCreate(
//     new Dictionary<string, object?>
//     {
//         ["first_name"] = "Jozo",
//         ["last_name"] = "Traktorista",
//         ["email"] = "jozo.traktorista@gmail.com",
//         ["password"] = "qwerty123"
//     }
// );
// Console.WriteLine($"UserCreate is \n {JsonConvert.SerializeObject(userCreate)}");

// OauthApi
var tokenResponse = await auth.Token(
    new OAuthInput { Identity = "jozo.traktorista@gmail.com", Password = "qwerty123" }
);

var tokenValid = await auth.Validate(tokenResponse.Data!.AccessToken.Token);
Console.WriteLine($"AccessToken is {(tokenValid ? "valid" : "not valid")}");

tokenResponse = await auth.TokenRefresh(
    tokenResponse.Data!.AccessToken.Token,
    tokenResponse.Data!.RefreshToken.Token
);
tokenValid = await auth.Validate(tokenResponse.Data!.AccessToken.Token);
Console.WriteLine($"AccessToken is {(tokenValid ? "valid" : "not valid")}");
