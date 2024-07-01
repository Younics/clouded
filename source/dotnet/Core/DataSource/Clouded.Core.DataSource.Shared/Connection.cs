using System.ComponentModel.DataAnnotations;

namespace Clouded.Core.DataSource.Shared;

public class Connection
{
    [Required]
    public DatabaseType Type { get; set; }

    [Required]
    public string Server { get; set; }

    [Required]
    public int Port { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
    public string Database { get; set; }
}
