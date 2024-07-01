using System.Net;
using Clouded.Results.Exceptions;

namespace Clouded.Auth.Provider.Exceptions;

public class BadCredentialsException()
    : CloudedException(
        "Entered credentials dont match our records",
        "auth.bad_credentials",
        (int)HttpStatusCode.BadRequest
    );
