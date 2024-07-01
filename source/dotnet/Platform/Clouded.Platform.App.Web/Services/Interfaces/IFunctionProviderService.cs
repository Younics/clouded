using Clouded.Platform.App.Web.Dtos;
using Clouded.Platform.Database.Entities.Clouded;

namespace Clouded.Platform.App.Web.Services.Interfaces;

public interface IFunctionProviderService
    : IProviderService<FunctionProviderEntity, FunctionProviderInput> { }
