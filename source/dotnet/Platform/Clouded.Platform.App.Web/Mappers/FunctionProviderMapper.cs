using AutoMapper;
using Clouded.Platform.App.Web.Dtos;
using Clouded.Platform.Database.Entities.Clouded;

namespace Clouded.Platform.App.Web.Mappers;

public class FunctionProviderMapper : Profile
{
    public FunctionProviderMapper()
    {
        CreateMap<FunctionProviderEntity, FunctionProviderInput>()
            .ForMember(d => d.RepositoryId, opt => opt.MapFrom(s => s.Configuration.RepositoryId))
            .ForMember(d => d.Branch, opt => opt.MapFrom(s => s.Configuration.Branch))
            .ForMember(
                d => d.RepositoryType,
                opt => opt.MapFrom(s => s.Configuration.RepositoryType)
            )
            .ForMember(
                d => d.Hooks,
                opt =>
                    opt.MapFrom(
                        s =>
                            s.Functions.Select(
                                x =>
                                    new FunctionProviderExecutableInput
                                    {
                                        ExecutableName = x.ExecutableName,
                                        FunctionName = x.Name,
                                        FunctionType = x.Type
                                    }
                            )
                    )
            );
    }
}
