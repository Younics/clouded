using Clouded.Platform.Database.Entities.Clouded;
using Clouded.Platform.Models.Dtos.Provider.Admin;

namespace Clouded.Platform.App.Web.Services.Interfaces;

public interface IAdminProviderService
    : IProviderService<AdminProviderEntity, AdminProviderInput> { }
