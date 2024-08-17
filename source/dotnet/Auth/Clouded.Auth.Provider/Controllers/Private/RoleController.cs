using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Clouded.Auth.Provider.Controllers.Private.Base;
using Clouded.Auth.Provider.Dictionaries;
using Clouded.Auth.Provider.Security;
using Clouded.Auth.Provider.Services.Interfaces;
using Clouded.Auth.Shared;
using Clouded.Auth.Shared.Pagination;
using Clouded.Auth.Shared.Role;
using Clouded.Results;
using Clouded.Results.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Clouded.Auth.Provider.Controllers.Private;

[ApiController]
[Route($"v1/{RoutesConfig.RolesRoutePrefix}")]
public class RoleController(IRoleService roleService) : BaseController
{
    private const string EntityName = "Role";

    /// <summary>
    /// Get all entities
    /// </summary>
    /// <param name="search"></param>
    /// <returns><see cref="IEnumerable{RoleDictionary}"/></returns>
    [CloudedAuthorize]
    [HttpGet]
    [SuccessCloudedResponse<IEnumerable<RoleDictionary>>]
    public IActionResult GetAll([FromQuery] string? search)
    {
        return Ok(roleService.GetAll(search));
    }

    /// <summary>
    /// Get all entities paginated 
    /// </summary>
    /// <param name="search"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <returns><see cref="Paginated{RoleDictionary}"/></returns>
    [CloudedAuthorize]
    [HttpGet("paginated")]
    [SuccessCloudedResponse<Paginated<RoleDictionary>>]
    public IActionResult GetAllPaginated(
        [FromQuery] string? search,
        [FromQuery] int page = 0,
        [FromQuery] int size = 50
    )
    {
        return Ok(roleService.GetAllPaginated(search, page, size));
    }

    /// <summary>
    /// Get entity by identity
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="withPermissions"></param>
    /// <returns><see cref="RoleDictionary"/></returns>
    [CloudedAuthorize]
    [HttpGet("by_identity/{identity}")]
    [SuccessCloudedResponse<RoleDictionary>]
    public IActionResult GetByIdentity(
        [Required] [FromRoute] string identity,
        [Optional] [FromQuery] bool withPermissions
    )
    {
        var machine = roleService.GetByIdentity(identity, withPermissions);
        return Ok(machine);
    }

    /// <summary>
    /// Get entity by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="withPermissions"></param>
    /// <returns><see cref="RoleDictionary"/></returns>
    [CloudedAuthorize]
    [HttpGet("{id}")]
    [SuccessCloudedResponse<RoleDictionary>]
    public virtual IActionResult GetById(
        [Required] [FromRoute] string id,
        [Optional] [FromQuery] bool withPermissions
    )
    {
        var entity = roleService.GetById(id, withPermissions);

        if (!entity.Any())
            throw new NotFoundException(EntityName);

        return Ok(entity);
    }

    /// <summary>
    /// Create entity
    /// </summary>
    /// <param name="data"></param>
    /// <returns><see cref="RoleDictionary"/></returns>
    [CloudedAuthorize]
    [HttpPost]
    [SuccessCloudedResponse<RoleDictionary>(StatusCodes.Status201Created)]
    public virtual IActionResult Create([Required] [FromBody] RoleDictionary data)
    {
        var entity = roleService.Create(data);

        return Created(Url.Action("GetById", EntityName, new { id = entity.Id }), entity);
    }

    /// <summary>
    /// Update entity
    /// </summary>
    /// <param name="id"></param>
    /// <param name="data"></param>
    /// <returns><see cref="RoleDictionary"/></returns>
    [CloudedAuthorize]
    [HttpPatch("{id}")]
    [SuccessCloudedResponse<RoleDictionary>]
    public virtual IActionResult Update(
        [Required] [FromRoute] string id,
        [Required] [FromBody] RoleDictionary data
    )
    {
        var entity = roleService.Update(id, data);

        return Ok(entity);
    }

    /// <summary>
    /// Delete entity
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [CloudedAuthorize]
    [HttpDelete("{id}")]
    public virtual IActionResult DeleteById([Required] [FromRoute] string id)
    {
        roleService.DeleteById(id);
        return Ok();
    }

    [CloudedAuthorize]
    [HttpPut("{id}/permissions/add")]
    public IActionResult AddPermissions(
        [Required] [FromRoute] string id,
        [Required] [FromBody] RolePermissionInput input
    )
    {
        roleService.AddPermissions(id, input);

        return Ok();
    }

    [CloudedAuthorize]
    [HttpPut("{id}/permissions/remove")]
    public IActionResult RemovePermissions(
        [Required] [FromRoute] string id,
        [Required] [FromBody] RolePermissionInput input
    )
    {
        roleService.RemovePermissions(id, input);

        return Ok();
    }
}
