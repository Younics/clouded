using System.ComponentModel.DataAnnotations;

namespace Clouded.Platform.App.Web.Options;

public class DatabaseOptions
{
    [Required]
    public CloudedConnectionOptions CloudedConnection { get; set; } = null!;
}
