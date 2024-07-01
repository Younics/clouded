using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clouded.Platform.Database.Entities.Base;
using Clouded.Platform.Models.Enums;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("providers")]
public abstract class ProviderEntity : TrackableEntity
{
    [StringLength(200)]
    [Column("name")]
    public string Name { get; set; } = null!;

    [StringLength(60)]
    [Column("code")]
    public string Code { get; set; } = null!;

    [Required]
    [Column("type")]
    public EProviderType Type { get; set; }

    [Required]
    [Column("status")]
    public EProviderStatus Status { get; set; }

    [Column("deployed_at")]
    public DateTime? DeployedAt { get; set; }

    [Column("domain_record_id")]
    public long? DomainRecordId { get; set; }

    [Column("domain_id")]
    public long? DomainId { get; set; }

    [Required]
    [Column("project_id")]
    public long ProjectId { get; set; }

    public virtual ProjectEntity Project { get; protected set; } = null!;
    public virtual DomainEntity? Domain { get; protected set; }

    public bool IsDeployed()
    {
        return DeployedAt >= Updated;
    }

    public string? GetDomainAddress()
    {
        if (Domain == null)
        {
            return null;
        }

        return "http://" + Code + "." + Domain.Value;
    }
}
