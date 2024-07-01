using Microsoft.EntityFrameworkCore;

namespace Clouded.Platform.Database.Seeds;

public static class Seeder
{
    public static void SeedAll(ModelBuilder builder)
    {
        RegionsSeeder.Seed(builder);
    }
}
