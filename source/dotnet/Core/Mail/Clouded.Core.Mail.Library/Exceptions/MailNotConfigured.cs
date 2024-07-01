using System.Net;
using Clouded.Results.Exceptions;

namespace Clouded.Core.Mail.Library.Exceptions;

public class MailNotConfiguredException()
    : CloudedException(
        "Mail is not configured!",
        "mail.not_configured",
        (int)HttpStatusCode.UnprocessableEntity
    );
