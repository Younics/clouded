namespace Clouded.Auth.Provider.Exceptions;

public class DomainNotAllowedException(string domain)
    : UnauthorizedException($"Domain {domain} is not allowed", "auth.domain_not_allowed");
