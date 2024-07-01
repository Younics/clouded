using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Clouded.Auth.Provider.Controllers.Private.Base;
using Clouded.Auth.Provider.Dictionaries;
using Clouded.Auth.Provider.Security;
using Clouded.Auth.Provider.Services.Interfaces;
using Clouded.Auth.Shared;
using Clouded.Auth.Shared.Organization;
using Clouded.Auth.Shared.Pagination;
using Clouded.Results;
using Clouded.Results.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Clouded.Auth.Provider.Controllers.Private;

[ApiController]
[Route($"v1/{RoutesConfig.OrganizationsRoutePrefix}")]
public class OrganizationController(IOrganizationService organizationService) : BaseController
{
    private const string EntityName = "Organization";

    /// <summary>
    ///
    /// </summary>
    /// <param name="search"></param>
    /// <returns><see cref="IEnumerable{OrganizationDictionary}"/></returns>
    [CloudedAuthorize]
    [HttpGet]
    [SuccessCloudedResponse<IEnumerable<UserDictionary>>]
    public IActionResult GetAll([FromQuery] string? search)
    {
        return Ok(organizationService.GetAll(search));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="search"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <returns><see cref="Paginated{OrganizationDictionary}"/></returns>
    [CloudedAuthorize]
    [HttpGet("paginated")]
    [SuccessCloudedResponse<Paginated<UserDictionary>>]
    public IActionResult GetAllPaginated(
        [FromQuery] string? search,
        [FromQuery] int page = 0,
        [FromQuery] int size = 50
    )
    {
        return Ok(organizationService.GetAllPaginated(search, page, size));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="withUsers"></param>
    /// <returns><see cref="OrganizationDictionary"/></returns>
    [CloudedAuthorize]
    [HttpGet("by_identity/{identity}")]
    [SuccessCloudedResponse<UserDictionary>]
    public IActionResult GetByIdentity(
        [Required] [FromRoute] string identity,
        [Optional] [FromQuery] bool withUsers
    )
    {
        var organization = organizationService.GetByIdentity(identity, withUsers);
        return Ok(organization);
    }

    /// <summary>
    /// Get entity by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="withUsers"></param>
    /// <returns><see cref="OrganizationDictionary"/></returns>
    [CloudedAuthorize]
    [HttpGet("{id}")]
    [SuccessCloudedResponse<Dictionary<string, object>>]
    public IActionResult GetById(
        [Required] [FromRoute] string id,
        [Optional] [FromQuery] bool withUsers
    )
    {
        var entity = organizationService.GetById(id!, withUsers);

        if (!entity.Any())
            throw new NotFoundException(EntityName);

        return Ok(entity);
    }

    /// <summary>
    /// Create entity
    /// </summary>
    /// <param name="data"></param>
    /// <returns><see cref="OrganizationDictionary"/></returns>
    [CloudedAuthorize]
    [HttpPost]
    [SuccessCloudedResponse<Dictionary<string, object>>(StatusCodes.Status201Created)]
    public IActionResult Create([Required] [FromBody] OrganizationDictionary data)
    {
        var entity = organizationService.Create(data);

        return Created(Url.Action("GetById", EntityName, new { id = entity.Id }), entity);
    }

    /// <summary>
    /// Update entity
    /// </summary>
    /// <param name="id"></param>
    /// <param name="data"></param>
    /// <returns><see cref="OrganizationDictionary"/></returns>
    [CloudedAuthorize]
    [HttpPatch("{id}")]
    [SuccessCloudedResponse<Dictionary<string, object>>]
    public IActionResult Update(
        [Required] [FromRoute] string id,
        [Required] [FromBody] OrganizationDictionary data
    )
    {
        var entity = organizationService.Update(id, data);

        return Ok(entity);
    }

    /// <summary>
    /// Delete entity
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [CloudedAuthorize]
    [HttpDelete("{id}")]
    public IActionResult DeleteById([Required] [FromRoute] string id)
    {
        organizationService.DeleteById(id);
        return Ok();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [CloudedAuthorize]
    [HttpPut("{id}/users/add")]
    public IActionResult AddUsers(
        [Required] [FromRoute] string id,
        [Required] [FromBody] OrganizationUserInput input
    )
    {
        organizationService.AddUsers(id, input);

        return Ok();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [CloudedAuthorize]
    [HttpPut("{id}/users/remove")]
    public IActionResult RemoveUsers(
        [Required] [FromRoute] string id,
        [Required] [FromBody] OrganizationUserInput input
    )
    {
        organizationService.RemoveUsers(id, input);

        return Ok();
    }
}
