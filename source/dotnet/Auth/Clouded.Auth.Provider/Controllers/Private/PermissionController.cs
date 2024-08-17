using System.ComponentModel.DataAnnotations;
using Clouded.Auth.Provider.Controllers.Private.Base;
using Clouded.Auth.Provider.DataSources.Interfaces;
using Clouded.Auth.Provider.Dictionaries;
using Clouded.Auth.Provider.Security;
using Clouded.Auth.Shared;
using Clouded.Results;
using Clouded.Results.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Clouded.Auth.Provider.Controllers.Private;

[ApiController]
[Route($"v1/{RoutesConfig.PermissionsRoutePrefix}")]
public class PermissionController(IPermissionDataSource permissionDataSource) : BaseController
{
    private const string EntityName = "Permission";

    /// <summary>
    /// Get all entities
    /// </summary>
    /// <param name="search"></param>
    /// <returns><see cref="IEnumerable{PermissionDictionary}"/></returns>
    [CloudedAuthorize]
    [HttpGet]
    [SuccessCloudedResponse<IEnumerable<PermissionDictionary>>]
    public IActionResult GetAll([FromQuery] string? search)
    {
        return Ok(permissionDataSource.Entities(search));
    }

    /// <summary>
    /// Get permission by identity
    /// </summary>
    /// <param name="identity"></param>
    /// <returns><see cref="PermissionDictionary"/></returns>
    [CloudedAuthorize]
    [HttpGet("by_identity/{identity}")]
    public virtual IActionResult GetByIdentity([Required] [FromRoute] string? identity)
    {
        var entity = permissionDataSource.EntityFindByIdentity(identity);

        if (!entity.Any())
            return NotFound();

        return Ok(entity);
    }

    /// <summary>
    /// Get entity by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns><see cref="PermissionDictionary"/></returns>
    [CloudedAuthorize]
    [HttpGet("{id}")]
    [SuccessCloudedResponse<Dictionary<string, object>>]
    public virtual IActionResult GetById([Required] [FromRoute] string id)
    {
        var entity = permissionDataSource.EntityFindById(id);

        if (!entity.Any())
            throw new NotFoundException(EntityName);

        return Ok(entity);
    }

    /// <summary>
    /// Create entity
    /// </summary>
    /// <param name="data"></param>
    /// <returns><see cref="PermissionDictionary"/></returns>
    [CloudedAuthorize]
    [HttpPost]
    [SuccessCloudedResponse<Dictionary<string, object>>(StatusCodes.Status201Created)]
    public virtual IActionResult Create([Required] [FromBody] PermissionDictionary data)
    {
        var entity = permissionDataSource.EntityCreate(data);

        return Created(Url.Action("GetById", EntityName, new { id = entity.Id }), entity);
    }

    /// <summary>
    /// Update entity
    /// </summary>
    /// <param name="id"></param>
    /// <param name="data"></param>
    /// <returns><see cref="PermissionDictionary"/></returns>
    [CloudedAuthorize]
    [HttpPatch("{id}")]
    [SuccessCloudedResponse<Dictionary<string, object>>]
    public virtual IActionResult Update(
        [Required] [FromRoute] string id,
        [Required] [FromBody] PermissionDictionary data
    )
    {
        var entity = permissionDataSource.EntityUpdate(id, data);

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
        permissionDataSource.EntityDelete(id);
        return Ok();
    }
}
