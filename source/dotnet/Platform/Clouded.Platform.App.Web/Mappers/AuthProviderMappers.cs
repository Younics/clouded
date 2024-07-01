using AutoMapper;
using Clouded.Platform.App.Web.Dtos;
using Clouded.Platform.Database.Entities.Clouded;
using Clouded.Platform.Models.Enums;

namespace Clouded.Platform.App.Web.Mappers;

public class AuthProviderMappers : Profile
{
    public AuthProviderMappers()
    {
        CreateMap<AuthSocialConfigurationEntity, AuthProviderSocialInput>().ReverseMap();

        CreateMap<AuthIdentityConfigurationEntity, AuthProviderIdentityInput>().ReverseMap();

        CreateMap<AuthIdentityRoleConfigurationEntity, AuthProviderIdentityInput>().ReverseMap();
        CreateMap<AuthIdentityOrganizationConfigurationEntity, AuthProviderIdentityInput>()
            .ReverseMap();
        CreateMap<AuthIdentityUserConfigurationEntity, AuthProviderIdentityUserInput>()
            .ReverseMap();
        CreateMap<AuthIdentityPermissionConfigurationEntity, AuthProviderIdentityInput>()
            .ReverseMap();

        CreateMap<AuthTokenConfigurationEntity, AuthProviderTokenInput>();

        CreateMap<AuthProviderTokenInput, AuthTokenConfigurationEntity>();

        CreateMap<AuthProviderEntity, AuthProviderInput>()
            .ForMember(x => x.DataSourceProviderId, opt => opt.MapFrom(src => src.DataSourceId))
            .ForMember(
                x => x.Code,
                opt => opt.MapFrom(src => src.Code.Replace($"{src.Project.Code}-auth-", ""))
            )
            .ForMember(x => x.CodePrefix, opt => opt.MapFrom(src => $"{src.Project.Code}-auth-"));

        CreateMap<AuthProviderInput, AuthProviderEntity>()
            .ForMember(x => x.DataSourceId, opt => opt.MapFrom(src => src.DataSourceProviderId))
            .ForMember(x => x.Code, opt => opt.MapFrom(src => $"{src.CodePrefix!}{src.Code}"));

        CreateMap<AuthProviderConfigurationInput, AuthConfigurationEntity>()
            .ForMember(x => x.Management, o => o.MapFrom(s => s.Management.Enabled))
            .ForMember(x => x.UserAccess, opt => opt.MapFrom(src => src.Management.Users))
            .ForMember(x => x.Hash, opt => opt.Ignore())
            .ForMember(
                x => x.IdentityOrganization,
                opt =>
                    opt.MapFrom(
                        src => src.IdentityOrganization.Enabled ? src.IdentityOrganization : null
                    )
            )
            .ForMember(
                x => x.IdentityUser,
                opt => opt.MapFrom(src => src.IdentityUser.Enabled ? src.IdentityUser : null)
            )
            .ForMember(
                x => x.IdentityRole,
                opt => opt.MapFrom(src => src.IdentityRole.Enabled ? src.IdentityRole : null)
            )
            .ForMember(
                x => x.IdentityPermission,
                opt =>
                    opt.MapFrom(
                        src => src.IdentityPermission.Enabled ? src.IdentityPermission : null
                    )
            )
            .ForMember(x => x.Mail, opt => opt.MapFrom(src => src.Mail.Enabled ? src.Mail : null));

        CreateMap<AuthConfigurationEntity, AuthProviderConfigurationInput>()
            .ForMember(x => x.Management, opt => opt.Ignore())
            .ForMember(x => x.Mail, o => o.MapFrom(s => s.Mail ?? new()))
            .ForMember(
                x => x.IdentityOrganization,
                o => o.MapFrom(s => s.IdentityOrganization ?? new())
            )
            .ForMember(x => x.IdentityRole, o => o.MapFrom(s => s.IdentityRole ?? new()))
            .ForMember(
                x => x.IdentityPermission,
                o => o.MapFrom(s => s.IdentityPermission ?? new())
            )
            .ForMember(x => x.Hash, opt => opt.Ignore())
            .ForMember(
                dest => dest.Facebook,
                opt =>
                    opt.MapFrom(
                        src =>
                            src.SocialConfiguration.FirstOrDefault(
                                i => i.Type == ESocialAuthType.Facebook,
                                new AuthSocialConfigurationEntity()
                            )
                    )
            )
            .ForMember(
                dest => dest.Google,
                opt =>
                    opt.MapFrom(
                        src =>
                            src.SocialConfiguration.FirstOrDefault(
                                i => i.Type == ESocialAuthType.Google,
                                new AuthSocialConfigurationEntity()
                            )
                    )
            )
            .ForMember(
                dest => dest.Apple,
                opt =>
                    opt.MapFrom(
                        src =>
                            src.SocialConfiguration.FirstOrDefault(
                                i => i.Type == ESocialAuthType.Apple,
                                new AuthSocialConfigurationEntity()
                            )
                    )
            );

        CreateMap<AuthIdentityUserConfigurationEntity, AuthProviderIdentityUserInput>()
            .ReverseMap();
        CreateMap<AuthMailConfigurationEntity, AuthProviderMailInput>().ReverseMap();
        CreateMap<AuthUserAccessEntity, AuthProviderManagementUserInput>().ReverseMap();

        CreateMap<AuthCorsConfigurationEntity, AuthProviderCorsInput>()
            .ForMember(
                dest => dest.AllowedMethods,
                opts =>
                    opts.MapFrom(
                        src => src.AllowedMethods.Split(",", StringSplitOptions.RemoveEmptyEntries)
                    )
            )
            .ForMember(
                dest => dest.AllowedOrigins,
                opts =>
                    opts.MapFrom(
                        src => src.AllowedOrigins.Split(",", StringSplitOptions.RemoveEmptyEntries)
                    )
            )
            .ForMember(
                dest => dest.AllowedHeaders,
                opts =>
                    opts.MapFrom(
                        src => src.AllowedHeaders.Split(",", StringSplitOptions.RemoveEmptyEntries)
                    )
            )
            .ForMember(
                dest => dest.ExposedHeaders,
                opts =>
                    opts.MapFrom(
                        src =>
                            src.ExposedHeaders != null
                                ? src.ExposedHeaders
                                    .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                    .ToList()
                                : new List<string>()
                    )
            );

        CreateMap<AuthProviderCorsInput, AuthCorsConfigurationEntity>()
            .ForMember(
                dest => dest.AllowedMethods,
                opts => opts.MapFrom(src => string.Join(",", src.AllowedMethods))
            )
            .ForMember(
                dest => dest.AllowedOrigins,
                opts => opts.MapFrom(src => string.Join(",", src.AllowedOrigins))
            )
            .ForMember(
                dest => dest.AllowedHeaders,
                opts => opts.MapFrom(src => string.Join(",", src.AllowedHeaders))
            )
            .ForMember(
                dest => dest.ExposedHeaders,
                opts => opts.MapFrom(src => string.Join(",", src.ExposedHeaders))
            );
    }
}
