using System.Linq.Expressions;
using System.Text.Json;
using AutoMapper;
using Clouded.Function.Shared;
using Clouded.Platform.Database.Entities.Clouded;
using Clouded.Platform.Models.Dtos.Provider.Admin;
using Clouded.Shared.Enums;

namespace Clouded.Platform.App.Web.Mappers;

public class AdminProviderMappers : Profile
{
    public AdminProviderMappers()
    {
        CreateMap<AdminUserAccessConfigurationEntity, AdminProviderUserAccessInput>()
            .ForMember(x => x.AdminProviderId, opt => opt.MapFrom(src => src.ProviderId))
            .ReverseMap();

        CreateMap<AdminNavigationGroupEntity, AdminProviderNavigationGroupInput>()
            .ForMember(x => x.AdminProviderId, opt => opt.MapFrom(src => src.ProviderId))
            .ReverseMap();

        CreateMap<AdminTablesConfigurationEntity, AdminProviderTableInput>()
            .ForMember(x => x.Enabled, opt => opt.MapFrom(y => true))
            .ForMember(x => x.AdminProviderId, opt => opt.MapFrom(src => src.ProviderId))
            .ForMember(
                x => x.Columns,
                opt =>
                    opt.MapFrom(
                        src =>
                            JsonSerializer.Deserialize<
                                List<AdminProviderTableInput.AdminProviderTableColumnInput>
                            >(src.Columns, JsonSerializerOptions.Default)
                    )
            )
            .ForMember(
                x => x.VirtualColumns,
                opt =>
                    opt.MapFrom(
                        src =>
                            JsonSerializer.Deserialize<
                                List<AdminProviderTableInput.AdminProviderTableColumnInput>
                            >(src.VirtualColumns, JsonSerializerOptions.Default)
                    )
            )
            .ForMember(
                x => x.CreateFunctions,
                opt =>
                    opt.MapFrom(MapToTableFunctionsBlockInput(EAdminProviderFunctionTrigger.Create))
            )
            .ForMember(
                x => x.UpdateFunctions,
                opt =>
                    opt.MapFrom(MapToTableFunctionsBlockInput(EAdminProviderFunctionTrigger.Update))
            )
            .ForMember(
                x => x.DeleteFunctions,
                opt =>
                    opt.MapFrom(MapToTableFunctionsBlockInput(EAdminProviderFunctionTrigger.Delete))
            );

        CreateMap<AdminProviderTableInput, AdminTablesConfigurationEntity>()
            .ForMember(x => x.ProviderId, opt => opt.MapFrom(src => src.AdminProviderId))
            .ForMember(
                x => x.Columns,
                opt =>
                    opt.MapFrom(
                        src => JsonSerializer.Serialize(src.Columns, JsonSerializerOptions.Default)
                    )
            )
            .ForMember(
                x => x.VirtualColumns,
                opt =>
                    opt.MapFrom(
                        src =>
                            JsonSerializer.Serialize(
                                src.VirtualColumns,
                                JsonSerializerOptions.Default
                            )
                    )
            );

        CreateMap<AdminProviderEntity, AdminProviderInput>()
            .ForMember(
                x => x.Code,
                opt => opt.MapFrom(src => src.Code.Replace($"{src.Project.Code}-admin-", ""))
            )
            .ForMember(x => x.CodePrefix, opt => opt.MapFrom(src => $"{src.Project.Code}-admin-"))
            .ForMember(
                x => x.DataSourceProviderIds,
                opt =>
                    opt.MapFrom(
                        src => src.DataSourcesRelation.Select(x => x.DataSourceId).AsEnumerable()
                    )
            )
            .ForMember(
                x => x.UserSettings,
                opts =>
                    opts.MapFrom(
                        src =>
                            new AdminProviderUserSettingsInput
                            {
                                DataSourceId =
                                    src.DataSourcesRelation.FirstOrDefault(
                                        i => i.HasUserSettingsTable
                                    ) != null
                                        ? src.DataSourcesRelation
                                            .First(i => i.HasUserSettingsTable)
                                            .DataSourceId
                                        : null,
                                UserSettingsSchema =
                                    src.DataSourcesRelation.FirstOrDefault(
                                        i => i.HasUserSettingsTable
                                    ) != null
                                        ? src.DataSourcesRelation
                                            .First(i => i.HasUserSettingsTable)
                                            .UserSettingsSchema
                                        : null,
                            }
                    )
            )
            .ForMember(
                x => x.CreateFunctions,
                opt => opt.MapFrom(MapToFunctionsBlockInput(EAdminProviderFunctionTrigger.Create))
            )
            .ForMember(
                x => x.UpdateFunctions,
                opt => opt.MapFrom(MapToFunctionsBlockInput(EAdminProviderFunctionTrigger.Update))
            )
            .ForMember(
                x => x.DeleteFunctions,
                opt => opt.MapFrom(MapToFunctionsBlockInput(EAdminProviderFunctionTrigger.Delete))
            );

        CreateMap<AdminProviderInput, AdminProviderEntity>()
            .ForMember(x => x.Tables, opt => opt.MapFrom(src => src.Tables.Where(z => z.Enabled)))
            .ForMember(
                x => x.DataSourcesRelation,
                opts =>
                    opts.MapFrom(
                        src =>
                            src.DataSourceProviderIds
                                .Select(
                                    id =>
                                        new AdminProviderDataSourceRelationEntity
                                        {
                                            DataSourceId = id,
                                            HasUserSettingsTable =
                                                id == src.UserSettings.DataSourceId,
                                            UserSettingsSchema =
                                                id == src.UserSettings.DataSourceId
                                                    ? src.UserSettings.UserSettingsSchema
                                                    : null
                                        }
                                )
                                .ToList()
                    )
            )
            .ForMember(x => x.Code, opt => opt.MapFrom(src => $"{src.CodePrefix!}{src.Code}"));
    }

    private static Expression<
        Func<AdminTablesConfigurationEntity, AdminProviderFunctionsBlockInput>
    > MapToTableFunctionsBlockInput(EAdminProviderFunctionTrigger trigger)
    {
        return src =>
            new AdminProviderFunctionsBlockInput
            {
                AfterHooks = src.FunctionsRelation
                    .Where(
                        i =>
                            i.OperationType == trigger && i.Function.Type == EFunctionType.AfterHook
                    )
                    .Select(i => i.FunctionId),
                BeforeHooks = src.FunctionsRelation
                    .Where(
                        i =>
                            i.OperationType == trigger
                            && i.Function.Type == EFunctionType.BeforeHook
                    )
                    .Select(i => i.FunctionId),
                InputHooks = src.FunctionsRelation
                    .Where(
                        i =>
                            i.OperationType == trigger && i.Function.Type == EFunctionType.InputHook
                    )
                    .Select(i => i.FunctionId),
                ValidationHooks = src.FunctionsRelation
                    .Where(
                        i =>
                            i.OperationType == trigger
                            && i.Function.Type == EFunctionType.ValidationHook
                    )
                    .Select(i => i.FunctionId)
            };
    }

    private static Expression<
        Func<AdminProviderEntity, AdminProviderFunctionsBlockInput>
    > MapToFunctionsBlockInput(EAdminProviderFunctionTrigger trigger)
    {
        return src =>
            new AdminProviderFunctionsBlockInput
            {
                AfterHooks = src.FunctionsRelation
                    .Where(
                        i =>
                            i.OperationType == trigger && i.Function.Type == EFunctionType.AfterHook
                    )
                    .Select(i => i.FunctionId),
                BeforeHooks = src.FunctionsRelation
                    .Where(
                        i =>
                            i.OperationType == trigger
                            && i.Function.Type == EFunctionType.BeforeHook
                    )
                    .Select(i => i.FunctionId),
                InputHooks = src.FunctionsRelation
                    .Where(
                        i =>
                            i.OperationType == trigger && i.Function.Type == EFunctionType.InputHook
                    )
                    .Select(i => i.FunctionId),
                ValidationHooks = src.FunctionsRelation
                    .Where(
                        i =>
                            i.OperationType == trigger
                            && i.Function.Type == EFunctionType.ValidationHook
                    )
                    .Select(i => i.FunctionId)
            };
    }
}
