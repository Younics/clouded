using System.ComponentModel.DataAnnotations;

namespace Clouded.Platform.Hub.Options;

public class DatabaseOptions
{
    [Required]
    public CloudedConnectionOptions CloudedConnection { get; set; }

    [Required]
    public ContainerConnectionOptions ContainerConnection { get; set; }
}
