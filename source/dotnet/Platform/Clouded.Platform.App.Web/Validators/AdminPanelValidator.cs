using Clouded.Platform.Models.Dtos.Provider.Admin;
using FluentValidation;

namespace Clouded.Platform.App.Web.Validators;

public class AdminPanelValidator : AbstractValidator<AdminProviderInput>
{
    public AdminPanelValidator()
    {
        RuleFor(entity => entity.Name).NotEmpty();
        RuleFor(entity => entity.Code).NotEmpty();
        RuleFor(entity => entity.DomainId).NotEmpty();
        RuleFor(entity => entity.DataSourceProviderIds).NotEmpty();

        RuleSet(
            "EditProfile",
            () =>
            {
                RuleFor(entity => entity.UserAccess).NotEmpty();
                RuleForEach(entity => entity.UserAccess)
                    .ChildRules(x =>
                    {
                        x.RuleFor(y => y.Identity).NotEmpty();
                        x.RuleFor(y => y.Password).NotEmpty();
                    });

                RuleFor(entity => entity.UserSettings)
                    .ChildRules(x =>
                    {
                        x.RuleFor(y => y.DataSourceId).NotEmpty();
                        x.RuleFor(y => y.UserSettingsSchema).NotEmpty();
                    });

                RuleFor(entity => entity.NavGroups)
                    .Must(x => x.Distinct(new AdminNavGroupOrderComparer()).Count() == x.Count())
                    .WithMessage("Order values are not unique.");
                RuleFor(entity => entity.NavGroups)
                    .Must(x => x.Distinct(new AdminNavGroupKeyComparer()).Count() == x.Count())
                    .WithMessage("Keys are not unique.");
                RuleForEach(entity => entity.NavGroups)
                    .ChildRules(x =>
                    {
                        x.RuleFor(y => y.Label).NotEmpty();
                        x.RuleFor(y => y.Key).NotEmpty();
                    });

                RuleFor(model => model.Tables)
                    .Custom(
                        (x, ctx) =>
                        {
                            var groupKeys = ctx.InstanceToValidate.NavGroups
                                .Select(w => w.Key)
                                .ToList();
                            var invalidTables = x.Where(
                                    i =>
                                        i is { Enabled: true, NavGroup: not null }
                                        && !groupKeys.Contains(i.NavGroup)
                                )
                                .ToList();
                            if (invalidTables.Any())
                            {
                                var tableNames = invalidTables.Select(g => $"{g.Schema}.{g.Table}");
                                ctx.AddFailure(
                                    "The Navigation group is not valid for tables: "
                                        + string.Join(", ", tableNames)
                                );
                            }
                        }
                    );

                RuleForEach(entity => entity.Tables)
                    .ChildRules(x =>
                    {
                        x.RuleFor(y => y.Schema).NotEmpty().When(y => y.Enabled);
                        x.RuleFor(y => y.Table).NotEmpty().When(y => y.Enabled);
                        x.RuleFor(y => y.Name).NotEmpty().When(y => y.Enabled);
                        x.RuleForEach(y => y.Columns)
                            .ChildRules(z =>
                            {
                                z.RuleFor(w => w.Name).NotEmpty().When(u => u.Enabled);
                                z.RuleFor(w => w.Column).NotEmpty().When(u => u.Enabled);
                                z.RuleFor(w => w.Type).IsInEnum().When(u => u.Enabled);
                                z.RuleFor(w => w.List).NotEmpty().When(u => u.Enabled);
                                z.RuleFor(w => w.Create).NotEmpty().When(u => u.Enabled);
                                z.RuleFor(w => w.Detail).NotEmpty().When(u => u.Enabled);
                            });

                        x.RuleFor(y => y.VirtualColumns)
                            .Must(
                                z => z.Distinct(new AdminVirtualColumnComparer()).Count() == z.Count
                            )
                            .WithMessage("Virtual column names are not unique.");

                        x.RuleForEach(y => y.VirtualColumns)
                            .ChildRules(z =>
                            {
                                z.RuleFor(w => w.Name).NotEmpty().When(u => u.Enabled);
                                z.RuleFor(w => w.Column).NotEmpty().When(u => u.Enabled);
                                z.RuleFor(w => w.VirtualType).IsInEnum().When(u => u.Enabled);
                                z.RuleFor(w => w.VirtualValue).IsInEnum().When(u => u.Enabled);
                                z.RuleFor(w => w.List).NotEmpty().When(u => u.Enabled);
                                z.RuleFor(w => w.Create).NotEmpty().When(u => u.Enabled);
                                z.RuleFor(w => w.Detail).NotEmpty().When(u => u.Enabled);
                            });
                    });
            }
        );
    }

    private class AdminNavGroupOrderComparer : IEqualityComparer<AdminProviderNavigationGroupInput>
    {
        public bool Equals(
            AdminProviderNavigationGroupInput x,
            AdminProviderNavigationGroupInput y
        ) => x?.Order == y?.Order;

        public int GetHashCode(AdminProviderNavigationGroupInput obj) => obj.Order;
    }

    private class AdminNavGroupKeyComparer : IEqualityComparer<AdminProviderNavigationGroupInput>
    {
        public bool Equals(
            AdminProviderNavigationGroupInput x,
            AdminProviderNavigationGroupInput y
        ) => x?.Key == y?.Key;

        public int GetHashCode(AdminProviderNavigationGroupInput obj) => obj.Order;
    }

    private class AdminVirtualColumnComparer
        : IEqualityComparer<AdminProviderTableInput.AdminProviderTableColumnInput>
    {
        public bool Equals(
            AdminProviderTableInput.AdminProviderTableColumnInput? x,
            AdminProviderTableInput.AdminProviderTableColumnInput? y
        ) => x?.Name == y?.Name;

        public int GetHashCode(AdminProviderTableInput.AdminProviderTableColumnInput obj) =>
            obj.GetHashCode();
    }
}
