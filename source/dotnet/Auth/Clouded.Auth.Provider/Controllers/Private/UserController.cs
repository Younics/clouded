using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Clouded.Auth.Provider.Controllers.Private.Base;
using Clouded.Auth.Provider.Dictionaries;
using Clouded.Auth.Provider.Security;
using Clouded.Auth.Provider.Services.Interfaces;
using Clouded.Auth.Shared;
using Clouded.Auth.Shared.Pagination;
using Clouded.Auth.Shared.User;
using Clouded.Results;
using Clouded.Results.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Clouded.Auth.Provider.Controllers.Private;

[ApiController]
[Route($"v1/{RoutesConfig.UsersRoutePrefix}")]
public class UserController(IUserService userService) : BaseController
{
    private const string EntityName = "User";

    /// <summary>
    ///
    /// </summary>
    /// <param name="search"></param>
    /// <param name="roleIds"></param>
    /// <param name="permissionIds"></param>
    /// <param name="organizationId"></param>
    /// <returns><see cref="IEnumerable{UserDictionary}"/></returns>
    [CloudedAuthorize]
    [HttpGet]
    [SuccessCloudedResponse<IEnumerable<UserDictionary>>]
    public IActionResult GetAll(
        [FromQuery] string? search,
        [FromQuery] IEnumerable<string>? roleIds,
        [FromQuery] IEnumerable<string>? permissionIds,
        [FromQuery] string? organizationId
    )
    {
        return Ok(userService.GetAll(search, roleIds, permissionIds, organizationId));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="search"></param>
    /// <param name="roleIds"></param>
    /// <param name="permissionIds"></param>
    /// <param name="organizationId"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <returns><see cref="Paginated{UserDictionary}"/></returns>
    [CloudedAuthorize]
    [HttpGet("paginated")]
    [SuccessCloudedResponse<Paginated<UserDictionary>>]
    public IActionResult GetAllPaginated(
        [FromQuery] string? search,
        [FromQuery] IEnumerable<string>? roleIds,
        [FromQuery] IEnumerable<string>? permissionIds,
        [FromQuery] string? organizationId,
        [FromQuery] int page = 0,
        [FromQuery] int size = 50
    )
    {
        return Ok(
            userService.GetAllPaginated(search, roleIds, permissionIds, organizationId, page, size)
        );
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="withRoles"></param>
    /// <param name="withPermissions"></param>
    /// <returns><see cref="UserDictionary"/></returns>
    [CloudedAuthorize]
    [HttpGet("by_identity/{identity}")]
    [SuccessCloudedResponse<UserDictionary>]
    public async Task<IActionResult> GetByIdentity(
        [Required] [FromRoute] string identity,
        [Optional] [FromQuery] bool withRoles,
        [Optional] [FromQuery] bool withPermissions
    )
    {
        var user = await userService.GetByIdentity(identity, withRoles, withPermissions);
        return Ok(user);
    }

    /// <summary>
    /// Get entity by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="withRoles"></param>
    /// <param name="withPermissions"></param>
    /// <returns><see cref="UserDictionary"/></returns>
    [CloudedAuthorize]
    [HttpGet("{id}")]
    [SuccessCloudedResponse<Dictionary<string, object>>]
    public virtual IActionResult GetById(
        [Required] [FromRoute] string id,
        [Optional] [FromQuery] bool withRoles,
        [Optional] [FromQuery] bool withPermissions
    )
    {
        var entity = userService.GetById(id!, withRoles, withPermissions);

        if (!entity.Any())
            throw new NotFoundException(EntityName);

        return Ok(entity);
    }

    /// <summary>
    /// Create entity
    /// </summary>
    /// <param name="data"></param>
    /// <returns><see cref="UserDictionary"/></returns>
    [CloudedAuthorize]
    [HttpPost]
    [SuccessCloudedResponse<UserDictionary>(StatusCodes.Status201Created)]
    public virtual IActionResult Create([Required] [FromBody] UserDictionary data)
    {
        var entity = userService.Create(data);

        return Created(Url.Action("GetById", EntityName, new { id = entity.Id }), entity);
    }

    /// <summary>
    /// Update entity
    /// </summary>
    /// <param name="id"></param>
    /// <param name="data"></param>
    /// <returns><see cref="UserDictionary"/></returns>
    [CloudedAuthorize]
    [HttpPatch("{id}")]
    [SuccessCloudedResponse<UserDictionary>]
    public virtual IActionResult Update(
        [Required] [FromRoute] string id,
        [Required] [FromBody] UserDictionary data
    )
    {
        var entity = userService.Update(id, data);

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
        userService.DeleteById(id);
        return Ok();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [CloudedAuthorize]
    [HttpPut("{id}/roles/add")]
    public IActionResult AddRoles(
        [Required] [FromRoute] string id,
        [Required] [FromBody] UserRoleInput input
    )
    {
        userService.AddRoles(id, input);

        return Ok();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [CloudedAuthorize]
    [HttpPut("{id}/roles/remove")]
    public IActionResult RemoveRoles(
        [Required] [FromRoute] string id,
        [Required] [FromBody] UserRoleInput input
    )
    {
        userService.RemoveRoles(id, input);

        return Ok();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [CloudedAuthorize]
    [HttpPut("{id}/permissions/add")]
    public IActionResult AddPermissions(
        [Required] [FromRoute] string id,
        [Required] [FromBody] UserPermissionInput input
    )
    {
        userService.AddPermissions(id, input);

        return Ok();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [CloudedAuthorize]
    [HttpPut("{id}/permissions/remove")]
    public IActionResult RemovePermissions(
        [Required] [FromRoute] string id,
        [Required] [FromBody] UserPermissionInput input
    )
    {
        userService.RemovePermissions(id, input);

        return Ok();
    }
}
