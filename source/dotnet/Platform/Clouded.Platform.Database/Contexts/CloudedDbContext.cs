using Clouded.Platform.Database.Contexts.Base;
using Clouded.Platform.Database.Entities.Clouded;
using Clouded.Platform.Database.Seeds;
using Clouded.Platform.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Clouded.Platform.Database.Contexts;

public class CloudedDbContext(DbContextOptions<CloudedDbContext> options) : DbContextBase(options)
{
    public DbSet<RegionEntity> Regions { get; set; } = null!;

    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<UserIntegrationEntity> UserIntegrations { get; set; } = null!;
    public DbSet<RoleEntity> Roles { get; set; } = null!;
    public DbSet<PermissionEntity> Permissions { get; set; } = null!;

    public DbSet<ProjectEntity> Projects { get; set; } = null!;

    public DbSet<DataSourceEntity> DataSources { get; set; } = null!;
    public DbSet<DataSourceConfigurationEntity> DataSourceConfigurations { get; set; } = null!;

    public DbSet<AuthProviderEntity> AuthProviders { get; set; } = null!;
    public DbSet<AuthConfigurationEntity> AuthConfigurations { get; set; } = null!;
    public DbSet<AuthSocialConfigurationEntity> AuthSocialConfigurations { get; set; } = null!;
    public DbSet<AuthCorsConfigurationEntity> AuthCorsConfigurations { get; set; } = null!;
    public DbSet<AuthUserAccessEntity> AuthUserAccessConfigurations { get; set; } = null!;
    public DbSet<AuthHashConfigurationEntity> AuthHashConfigurations { get; set; } = null!;
    public DbSet<AuthHashArgon2ConfigurationEntity> AuthHashArgon2Configurations { get; set; } =
        null!;
    public DbSet<AuthTokenConfigurationEntity> AuthTokenConfigurations { get; set; } = null!;
    public DbSet<AuthIdentityOrganizationConfigurationEntity> AuthIdentityOrganizationConfigurations { get; set; } =
        null!;
    public DbSet<AuthIdentityUserConfigurationEntity> AuthIdentityUserConfigurations { get; set; } =
        null!;
    public DbSet<AuthIdentityRoleConfigurationEntity> AuthIdentityRoleConfigurations { get; set; } =
        null!;
    public DbSet<AuthIdentityPermissionConfigurationEntity> AuthIdentityPermissionConfigurations { get; set; } =
        null!;

    public DbSet<AdminProviderEntity> AdminProviders { get; set; } = null!;
    public DbSet<AdminConfigurationEntity> AdminConfigurations { get; set; } = null!;
    public DbSet<AdminTablesConfigurationEntity> AdminTablesConfigurations { get; set; } = null!;
    public DbSet<AdminUserAccessConfigurationEntity> AdminUserAccessConfiguration { get; set; } =
        null!;
    public DbSet<AdminNavigationGroupEntity> AdminNavigationGroupConfiguration { get; set; } =
        null!;

    public DbSet<FunctionProviderEntity> FunctionProviders { get; set; } = null!;
    public DbSet<FunctionConfigurationEntity> FunctionConfigurations { get; set; } = null!;
    public DbSet<FunctionEntity> FunctionHookConfigurations { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<RegionEntity>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();

            entity.Property(x => x.Code).HasConversion<string>();
        });

        builder.TrackableEntity<UserEntity>(entity => { });

        builder.TrackableEntity<UserIntegrationEntity>(entity =>
        {
            entity
                .HasOne(x => x.User)
                .WithOne(x => x.Integration)
                .HasForeignKey<UserIntegrationEntity>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.TrackableEntity<ProjectEntity>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();

            entity
                .HasMany(x => x.Users)
                .WithMany(x => x.Projects)
                .UsingEntity<ProjectUserRelationEntity>(
                    r =>
                        r.HasOne(x => x.User)
                            .WithMany(x => x.ProjectsRelation)
                            .HasForeignKey(x => x.UserId),
                    l =>
                        l.HasOne(x => x.Project)
                            .WithMany(x => x.UsersRelation)
                            .HasForeignKey(x => x.ProjectId),
                    j =>
                    {
                        j.HasKey(x => new { x.UserId, x.ProjectId });
                    }
                );

            entity
                .HasMany(x => x.DataSources)
                .WithOne(x => x.Project)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasMany(x => x.Domains)
                .WithOne(x => x.Project)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasMany(x => x.AuthProviders)
                .WithOne(x => x.Project)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasMany(x => x.AdminProviders)
                .WithOne(x => x.Project)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasMany(x => x.FunctionProviders)
                .WithOne(x => x.Project)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(x => x.Region)
                .WithMany()
                .HasForeignKey(x => x.RegionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.TrackableEntity<DomainEntity>(entity =>
        {
            entity
                .HasMany(x => x.AuthProviders)
                .WithOne(x => x.Domain)
                .HasForeignKey(x => x.DomainId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity
                .HasMany(x => x.AdminProviders)
                .WithOne(x => x.Domain)
                .HasForeignKey(x => x.DomainId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity
                .HasMany(x => x.FunctionProviders)
                .WithOne(x => x.Domain)
                .HasForeignKey(x => x.DomainId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        builder.TrackableEntity<DataSourceEntity>(entity =>
        {
            entity
                .HasOne(x => x.Configuration)
                .WithOne(x => x.DataSource)
                .HasForeignKey<DataSourceConfigurationEntity>(x => x.DataSourceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasMany(x => x.UsingAdminTablesConfiguration)
                .WithOne(x => x.DataSource)
                .HasForeignKey(x => x.DataSourceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.TrackableEntity<DataSourceConfigurationEntity>(entity =>
        {
            entity.Property(x => x.Type).HasConversion<string>();
        });

        builder.TrackableEntity<AdminTablesConfigurationEntity>(entity =>
        {
            entity
                .HasMany(x => x.Functions)
                .WithMany(x => x.UsingAdminProviderTables)
                .UsingEntity<AdminProviderTableFunctionRelationEntity>(
                    r =>
                        r.HasOne(x => x.Function)
                            .WithMany(x => x.UsingAdminProviderTablesRelation)
                            .HasForeignKey(x => x.FunctionId),
                    l =>
                        l.HasOne(x => x.AdminProviderTable)
                            .WithMany(x => x.FunctionsRelation)
                            .HasForeignKey(x => x.AdminProviderTableId),
                    j =>
                    {
                        j.HasKey(
                            x =>
                                new
                                {
                                    HookId = x.FunctionId,
                                    x.AdminProviderTableId,
                                    x.OperationType
                                }
                        );
                    }
                );
        });

        builder.TrackableEntity<ProviderEntity>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();

            entity.Property(x => x.Type).HasConversion<string>();

            entity.Property(x => x.Status).HasConversion<string>();

            entity
                .HasDiscriminator(x => x.Type)
                .HasValue<AuthProviderEntity>(EProviderType.Auth)
                .HasValue<AdminProviderEntity>(EProviderType.Admin)
                // .HasValue<ApiProviderEntity>(EProviderType.Api)
                .HasValue<FunctionProviderEntity>(EProviderType.Function);
        });

        builder.TrackableEntity<AuthProviderEntity>(entity =>
        {
            entity
                .HasOne(x => x.Configuration)
                .WithOne(x => x.Provider)
                .HasForeignKey<AuthConfigurationEntity>(x => x.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(x => x.DataSource)
                .WithMany()
                .HasForeignKey(x => x.DataSourceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasMany(x => x.Hooks)
                .WithMany(x => x.UsingAuthProviders)
                .UsingEntity<AuthProviderFunctionRelationEntity>(
                    r =>
                        r.HasOne(x => x.Function)
                            .WithMany(x => x.UsingAuthProvidersRelation)
                            .HasForeignKey(x => x.FunctionId),
                    l =>
                        l.HasOne(x => x.AuthProvider)
                            .WithMany(x => x.HooksRelation)
                            .HasForeignKey(x => x.AuthProviderId),
                    j =>
                    {
                        j.HasKey(x => new { HookId = x.FunctionId, x.AuthProviderId });
                    }
                );
        });

        builder.TrackableEntity<AuthConfigurationEntity>(entity =>
        {
            entity
                .HasOne(x => x.Provider)
                .WithOne(x => x.Configuration)
                .HasForeignKey<AuthConfigurationEntity>(x => x.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasMany(x => x.SocialConfiguration)
                .WithOne(x => x.Configuration)
                .HasForeignKey(x => x.ConfigurationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasMany(x => x.UserAccess)
                .WithOne(x => x.Configuration)
                .HasForeignKey(x => x.ConfigurationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.TrackableEntity<AuthHashConfigurationEntity>(entity =>
        {
            entity.Property(x => x.AlgorithmType).HasConversion<string>();

            entity
                .HasDiscriminator(x => x.AlgorithmType)
                .HasValue<AuthHashArgon2ConfigurationEntity>(EHashType.Argon2);

            entity
                .HasOne(x => x.Configuration)
                .WithOne(x => x.Hash)
                .HasForeignKey<AuthHashConfigurationEntity>(x => x.ConfigurationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.TrackableEntity<AuthCorsConfigurationEntity>(entity =>
        {
            entity
                .HasOne(x => x.Configuration)
                .WithOne(x => x.Cors)
                .HasForeignKey<AuthCorsConfigurationEntity>(x => x.ConfigurationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.TrackableEntity<AuthMailConfigurationEntity>(entity =>
        {
            entity
                .HasOne(x => x.Configuration)
                .WithOne(x => x.Mail)
                .HasForeignKey<AuthMailConfigurationEntity>(x => x.ConfigurationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.TrackableEntity<AuthTokenConfigurationEntity>(entity =>
        {
            entity
                .HasOne(x => x.Configuration)
                .WithOne(x => x.Token)
                .HasForeignKey<AuthTokenConfigurationEntity>(x => x.ConfigurationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.TrackableEntity<AuthIdentityConfigurationEntity>(entity =>
        {
            entity.Property(x => x.IdentityType).HasConversion<string>();

            entity
                .HasDiscriminator(x => x.IdentityType)
                .HasValue<AuthIdentityOrganizationConfigurationEntity>(EIdentityType.Organization)
                .HasValue<AuthIdentityUserConfigurationEntity>(EIdentityType.User)
                .HasValue<AuthIdentityRoleConfigurationEntity>(EIdentityType.Role)
                .HasValue<AuthIdentityPermissionConfigurationEntity>(EIdentityType.Permission);
        });

        builder
            .Entity<AuthIdentityOrganizationConfigurationEntity>()
            .HasOne(x => x.Configuration)
            .WithOne(x => x.IdentityOrganization)
            .HasForeignKey<AuthIdentityOrganizationConfigurationEntity>(x => x.ConfigurationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Entity<AuthIdentityUserConfigurationEntity>()
            .HasOne(x => x.Configuration)
            .WithOne(x => x.IdentityUser)
            .HasForeignKey<AuthIdentityUserConfigurationEntity>(x => x.ConfigurationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Entity<AuthIdentityRoleConfigurationEntity>()
            .HasOne(x => x.Configuration)
            .WithOne(x => x.IdentityRole)
            .HasForeignKey<AuthIdentityRoleConfigurationEntity>(x => x.ConfigurationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Entity<AuthIdentityPermissionConfigurationEntity>()
            .HasOne(x => x.Configuration)
            .WithOne(x => x.IdentityPermission)
            .HasForeignKey<AuthIdentityPermissionConfigurationEntity>(x => x.ConfigurationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.TrackableEntity<AdminProviderEntity>(entity =>
        {
            entity
                .HasOne(x => x.Configuration)
                .WithOne(x => x.Provider)
                .HasForeignKey<AdminConfigurationEntity>(x => x.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasMany(x => x.DataSources)
                .WithMany(x => x.UsingAdminProviders)
                .UsingEntity<AdminProviderDataSourceRelationEntity>(
                    r =>
                        r.HasOne(x => x.DataSource)
                            .WithMany(x => x.UsingAdminProvidersRelation)
                            .HasForeignKey(x => x.DataSourceId),
                    l =>
                        l.HasOne(x => x.AdminProvider)
                            .WithMany(x => x.DataSourcesRelation)
                            .HasForeignKey(x => x.AdminProviderId),
                    j =>
                    {
                        j.HasKey(x => new { x.DataSourceId, x.AdminProviderId });
                    }
                );

            entity
                .HasMany(x => x.UserAccess)
                .WithOne(x => x.Provider)
                .HasForeignKey(x => x.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasMany(x => x.Tables)
                .WithOne(x => x.Provider)
                .HasForeignKey(x => x.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasMany(x => x.NavGroups)
                .WithOne(x => x.Provider)
                .HasForeignKey(x => x.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasMany(x => x.Functions)
                .WithMany(x => x.UsingAdminProviders)
                .UsingEntity<AdminProviderFunctionRelationEntity>(
                    r =>
                        r.HasOne(x => x.Function)
                            .WithMany(x => x.UsingAdminProvidersRelation)
                            .HasForeignKey(x => x.FunctionId),
                    l =>
                        l.HasOne(x => x.AdminProvider)
                            .WithMany(x => x.FunctionsRelation)
                            .HasForeignKey(x => x.AdminProviderId),
                    j =>
                    {
                        j.HasKey(
                            x =>
                                new
                                {
                                    HookId = x.FunctionId,
                                    x.AdminProviderId,
                                    x.OperationType
                                }
                        );
                    }
                );
        });

        builder.TrackableEntity<FunctionProviderEntity>(entity =>
        {
            entity
                .HasOne(x => x.Configuration)
                .WithOne(x => x.Provider)
                .HasForeignKey<FunctionConfigurationEntity>(x => x.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasMany(x => x.Functions)
                .WithOne(x => x.Provider)
                .HasForeignKey(x => x.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.TrackableEntity<FunctionConfigurationEntity>(entity =>
        {
            entity.Property(x => x.RepositoryType).HasConversion<string>();
        });

        builder.TrackableEntity<FunctionEntity>(entity =>
        {
            entity.Property(x => x.Type).HasConversion<string>();
        });

        Seeder.SeedAll(builder);
    }
}
