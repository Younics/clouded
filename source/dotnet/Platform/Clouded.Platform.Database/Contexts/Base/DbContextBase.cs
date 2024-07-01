using Clouded.Platform.Database.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace Clouded.Platform.Database.Contexts.Base;

public abstract class DbContextBase(DbContextOptions options) : DbContext(options)
{
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        BeforeSaving();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default
    )
    {
        BeforeSaving();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void BeforeSaving()
    {
        var entries = ChangeTracker.Entries();

        foreach (var entry in entries)
        {
            if (entry.Entity is ITrackableEntity trackableEntity)
            {
                var now = DateTime.UtcNow;

                switch (entry.State)
                {
                    case EntityState.Modified:
                        trackableEntity.Updated ??= now;
                        break;

                    case EntityState.Added:
                        break;
                    case EntityState.Deleted:
                        break;
                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
