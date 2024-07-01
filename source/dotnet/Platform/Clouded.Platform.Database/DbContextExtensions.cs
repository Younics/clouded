using System.Linq.Expressions;
using Clouded.Platform.Database.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;

namespace Clouded.Platform.Database;

public static class DbContextExtensions
{
    public static ModelBuilder TrackableEntity<TEntity>(
        this ModelBuilder builder,
        Action<EntityTypeBuilder<TEntity>> buildAction
    )
        where TEntity : TrackableEntity
    {
        builder.Entity<TEntity>().Property(p => p.Created).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Entity<TEntity>().Property(p => p.Updated).HasDefaultValueSql("CURRENT_TIMESTAMP");
        buildAction(builder.Entity<TEntity>());

        return builder;
    }

    public static void TransactionStart(this DbContext context) =>
        context.Database.BeginTransaction();

    public static async Task<IDbContextTransaction> TransactionStartAsync(
        this DbContext context,
        CancellationToken cancellationToken = default
    ) => await context.Database.BeginTransactionAsync(cancellationToken);

    public static void LazyLoad<TEntity, TProperty>(
        this DbContext context,
        TEntity entity,
        Expression<Func<TEntity, TProperty?>> propertyExpression
    )
        where TEntity : class, IEntity, new()
        where TProperty : class
    {
        var entry = context.Entry(entity);
        entry.Reference(propertyExpression).Load();
    }

    public static async Task LazyLoadAsync<TEntity, TProperty>(
        this DbContext context,
        TEntity entity,
        Expression<Func<TEntity, TProperty?>> propertyExpression,
        CancellationToken cancellationToken = default
    )
        where TEntity : class, IEntity, new()
        where TProperty : class
    {
        var entry = context.Entry(entity);
        await entry.Reference(propertyExpression).LoadAsync(cancellationToken);
    }

    public static int Count<TEntity>(this DbContext context)
        where TEntity : class, IEntity, new() => context.Set<TEntity>().Count();

    public static int Count<TEntity>(this DbContext context, Func<TEntity, bool> expression)
        where TEntity : class, IEntity, new() => context.Set<TEntity>().Count(expression);

    public static Task<int> CountAsync<TEntity>(
        this DbContext context,
        CancellationToken cancellationToken = default
    )
        where TEntity : class, IEntity, new() =>
        context.Set<TEntity>().CountAsync(cancellationToken);

    public static Task<int> CountAsync<TEntity>(
        this DbContext context,
        Expression<Func<TEntity, bool>> expression,
        CancellationToken cancellationToken = default
    )
        where TEntity : class, IEntity, new() =>
        context.Set<TEntity>().CountAsync(expression, cancellationToken);

    public static bool Any<TEntity>(this DbContext context, Func<TEntity, bool> expression)
        where TEntity : class, IEntity, new() => context.Set<TEntity>().Any(expression);

    public static Task<bool> AnyAsync<TEntity>(
        this DbContext context,
        Expression<Func<TEntity, bool>> expression,
        CancellationToken cancellationToken = default
    )
        where TEntity : class, IEntity, new() =>
        context.Set<TEntity>().AnyAsync(expression, cancellationToken);

    public static Task<bool> AnyAsync<TEntity>(
        this DbContext context,
        long id,
        CancellationToken cancellationToken = default
    )
        where TEntity : class, IEntity, new() =>
        context.Set<TEntity>().AnyAsync(x => x.Id == id, cancellationToken);

    public static IQueryable<TEntity> GetAll<TEntity>(this DbContext context)
        where TEntity : class, IEntity, new() => context.Set<TEntity>().AsQueryable();

    public static (Task<int> TotalCountAsync, IQueryable<TEntity> Items) GetPaginated<TEntity>(
        this DbContext context,
        Expression<Func<TEntity, bool>>? filterExpression = default,
        Expression<Func<TEntity, object>>? orderExpression = default,
        bool orderDescending = false,
        int pageIndex = 0,
        int pageSize = 25,
        CancellationToken cancellationToken = default
    )
        where TEntity : class, IEntity, new()
    {
        var items = context.GetAll<TEntity>();

        if (filterExpression != null)
            items = items.Where(filterExpression);

        if (orderExpression != null)
            items = orderDescending
                ? items.OrderByDescending(orderExpression)
                : items.OrderBy(orderExpression);

        var totalCountAsync = items.CountAsync(cancellationToken);

        return (
            TotalCountAsync: totalCountAsync,
            Items: items.Skip(pageIndex * pageSize).Take(pageSize)
        );
    }

    public static IQueryable<TEntity> GetByIds<TEntity>(
        this DbContext context,
        IEnumerable<long> ids
    )
        where TEntity : class, IEntity, new() =>
        context.Set<TEntity>().AsQueryable().Where(x => ids.Contains(x.Id));

    public static TEntity? Get<TEntity>(
        this DbContext context,
        Expression<Func<TEntity, bool>> expression
    )
        where TEntity : class, IEntity, new() =>
        context.Set<TEntity>().AsQueryable().FirstOrDefault(expression);

    public static TEntity? Get<TEntity>(this DbContext context, long id)
        where TEntity : class, IEntity, new() => context.Get<TEntity>(x => x.Id == id);

    public static Task<TEntity?> GetAsync<TEntity>(
        this DbContext context,
        Expression<Func<TEntity, bool>> expression,
        CancellationToken cancellationToken = default
    )
        where TEntity : class, IEntity, new() =>
        context.Set<TEntity>().AsQueryable().FirstOrDefaultAsync(expression, cancellationToken);

    public static Task<TEntity?> GetAsync<TEntity>(
        this DbContext context,
        long id,
        CancellationToken cancellationToken = default
    )
        where TEntity : class, IEntity, new() =>
        context.GetAsync<TEntity>(x => x.Id == id, cancellationToken);

    public static void Create<TEntity>(this DbContext context, TEntity entity)
        where TEntity : class, IEntity, new() => context.Set<TEntity>().Add(entity);

    public static async Task CreateAsync<TEntity>(
        this DbContext context,
        TEntity entity,
        CancellationToken cancellationToken = default
    )
        where TEntity : class, IEntity, new() =>
        await context.Set<TEntity>().AddAsync(entity, cancellationToken);

    public static void CreateBulk<TEntity>(this DbContext context, IEnumerable<TEntity> entities)
        where TEntity : class, IEntity, new() => context.Set<TEntity>().AddRange(entities);

    public static async Task CreateBulkAsync<TEntity>(
        this DbContext context,
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default
    )
        where TEntity : class, IEntity, new() =>
        await context.Set<TEntity>().AddRangeAsync(entities, cancellationToken);

    public static void Update<TEntity>(this DbContext context, long id, Action<TEntity> updateFunc)
        where TEntity : class, IEntity, new()
    {
        var entity = context.Get<TEntity>(id);

        if (entity == null)
            throw new NullReferenceException();

        updateFunc(entity);
    }

    public static async Task<TEntity> UpdateAsync<TEntity>(
        this DbContext context,
        long id,
        Action<TEntity> updateFunc,
        CancellationToken cancellationToken = default
    )
        where TEntity : class, IEntity, new()
    {
        var entity = await context.GetAsync<TEntity>(id, cancellationToken);

        if (entity == null)
            throw new NullReferenceException();

        updateFunc(entity);
        return entity;
    }

    public static void Delete<TEntity>(this DbContext context, long id)
        where TEntity : class, IEntity, new()
    {
        var entity = context.Get<TEntity>(id);

        if (entity != null)
            context.Set<TEntity>().Remove(entity);
    }

    public static void Delete<TEntity>(this DbContext context, TEntity? entity)
        where TEntity : class, IEntity, new()
    {
        if (entity == null)
            return;

        context.Set<TEntity>().Remove(entity);
    }

    public static void DeleteAll<TEntity>(this DbContext context, IEnumerable<TEntity> entities)
        where TEntity : class, IEntity, new() => context.Set<TEntity>().RemoveRange(entities);

    public static void DeleteAll<TEntity>(this DbContext context, Func<TEntity, bool> expression)
        where TEntity : class, IEntity, new()
    {
        var entities = context.Set<TEntity>().Where(expression);
        context.DeleteAll(entities);
    }

    public static void SaveChanges(this DbContext context) => context.SaveChanges();

    public static async Task SaveChangesAsync(
        this DbContext context,
        CancellationToken cancellationToken = default
    ) => await context.SaveChangesAsync(cancellationToken);
}
