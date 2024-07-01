using System.ComponentModel.DataAnnotations;

namespace Clouded.Platform.App.Web.Options;

public class WebSupportOptions
{
    [Required]
    public string ApiKey { get; set; } = null!;

    [Required]
    public string Secret { get; set; } = null!;
}
