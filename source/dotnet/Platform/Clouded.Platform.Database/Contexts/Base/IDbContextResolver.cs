using Microsoft.EntityFrameworkCore;

namespace Clouded.Platform.Database.Contexts.Base;

public interface IDbContextResolver
{
    public TContext UseScopedContext<TContext>()
        where TContext : DbContext;
    public TContext MakeContext<TContext>()
        where TContext : DbContext;
}
