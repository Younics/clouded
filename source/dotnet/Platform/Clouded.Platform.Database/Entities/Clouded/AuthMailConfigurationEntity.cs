using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clouded.Platform.Database.Entities.Base;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("configurations_auth_mail")]
public class AuthMailConfigurationEntity : TrackableEntity
{
    [Required]
    [Column("from")]
    public string From { get; set; }

    [Required]
    [Column("host")]
    public string Host { get; set; }

    [Required]
    [Column("port")]
    public int Port { get; set; }

    [Required]
    [Column("reset_password_return_url")]
    public string ResetPasswordReturnUrl { get; set; } = "";

    [Column("user")]
    public string? User { get; set; }

    [Column("password")]
    public string? Password { get; set; }

    [Column("socket_options")]
    public string? SocketOptions { get; set; }

    [Required]
    [Column("authentication")]
    public bool Authentication { get; set; }

    [Required]
    [Column("use_ssl")]
    public bool UseSsl { get; set; }

    [Required]
    [Column("configuration_id")]
    public long ConfigurationId { get; set; }

    public virtual AuthConfigurationEntity Configuration { get; protected set; } = null!;
}
