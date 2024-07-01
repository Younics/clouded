using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clouded.Platform.Database.Entities.Base;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("configurations_admin_navigation_groups")]
public class AdminNavigationGroupEntity : TrackableEntity
{
    [Required]
    [Column("key")]
    public string Key { get; set; } = null!;

    [Required]
    [Column("label")]
    public string Label { get; set; } = null!;

    [Column("icon")]
    public string? Icon { get; set; } = null!;

    [Required]
    [Column("order")]
    public int Order { get; set; }

    [Required]
    [Column("provider_id")]
    public long ProviderId { get; set; }

    public virtual AdminProviderEntity Provider { get; protected set; } = null!;
}
