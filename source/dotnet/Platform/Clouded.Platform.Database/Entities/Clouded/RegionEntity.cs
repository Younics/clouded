using System.ComponentModel.DataAnnotations.Schema;
using Clouded.Platform.Database.Entities.Base;
using Clouded.Platform.Models.Enums;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("regions")]
public class RegionEntity : Entity
{
    [Column("code")]
    public ERegionCode Code { get; set; }

    [Column("city")]
    public string City { get; set; } = null!;

    [Column("state")]
    public string State { get; set; } = null!;

    [Column("continent")]
    public string Continent { get; set; } = null!;
}
