using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Clouded.Platform.Database.Contexts.Base;

public class DbContextResolver(IServiceProvider serviceProvider) : IDbContextResolver
{
    private DbContext? Context { get; set; }

    public TContext UseScopedContext<TContext>()
        where TContext : DbContext
    {
        Context ??= MakeContext<TContext>();
        return (TContext)Context;
    }

    public TContext MakeContext<TContext>()
        where TContext : DbContext =>
        serviceProvider.GetRequiredService<IDbContextFactory<TContext>>().CreateDbContext();
}
