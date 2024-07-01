using Clouded.Function.Library.Options;

namespace Clouded.Auth.Provider.Options;

public class CloudedOptions
{
    public required LicenseOptions License { get; set; }
    public required String Domain { get; set; }
    public required bool SwaggerEnabled { get; set; }
    public required DataSourceOptions DataSource { get; set; }

    public required AuthOptions Auth { get; set; }

    public FunctionOptions? Function { get; set; }

    public MailOptions? Mail { get; set; }
}
