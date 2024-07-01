using Clouded.Platform.Database.Entities.Clouded;
using Clouded.Platform.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Clouded.Platform.Database.Seeds;

public static class RegionsSeeder
{
    public static void Seed(ModelBuilder builder)
    {
        builder
            .Entity<RegionEntity>()
            .HasData(
                new RegionEntity
                {
                    Id = 1,
                    Code = ERegionCode.EuSkBa,
                    Continent = "Europe",
                    State = "Slovakia",
                    City = "Bratislava"
                }
            );
    }
}
