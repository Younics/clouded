using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Clouded.Auth.Provider.Controllers.Private.Base;
using Clouded.Auth.Provider.Dictionaries;
using Clouded.Auth.Provider.Security;
using Clouded.Auth.Provider.Services.Interfaces;
using Clouded.Auth.Shared;
using Clouded.Auth.Shared.Machine;
using Clouded.Auth.Shared.Pagination;
using Clouded.Results;
using Clouded.Results.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Clouded.Auth.Provider.Controllers.Private;

[ApiController]
[Route($"v1/{RoutesConfig.MachinesRoutePrefix}")]
public class MachineController(IMachineService machineService) : BaseController
{
    private const string EntityName = "Machine";

    /// <summary>
    ///
    /// </summary>
    /// <param name="search"></param>
    /// <param name="roleIds"></param>
    /// <param name="permissionIds"></param>
    /// <returns><see cref="IEnumerable{MachineDictionary}"/></returns>
    [CloudedAuthorize]
    [HttpGet]
    [SuccessCloudedResponse<IEnumerable<MachineDictionary>>]
    public IActionResult GetAll(
        [FromQuery] string? search,
        [FromQuery] IEnumerable<string>? roleIds,
        [FromQuery] IEnumerable<string>? permissionIds
    )
    {
        return Ok(machineService.GetAll(search, roleIds, permissionIds));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="search"></param>
    /// <param name="roleIds"></param>
    /// <param name="permissionIds"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <returns><see cref="Paginated{MachineDictionary}"/></returns>
    [CloudedAuthorize]
    [HttpGet("paginated")]
    [SuccessCloudedResponse<Paginated<MachineDictionary>>]
    public IActionResult GetAllPaginated(
        [FromQuery] string? search,
        [FromQuery] IEnumerable<string>? roleIds,
        [FromQuery] IEnumerable<string>? permissionIds,
        [FromQuery] int page = 0,
        [FromQuery] int size = 50
    )
    {
        return Ok(machineService.GetAllPaginated(search, roleIds, permissionIds, page, size));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="withRoles"></param>
    /// <param name="withPermissions"></param>
    /// <param name="withDomains"></param>
    /// <returns><see cref="MachineDictionary"/></returns>
    [CloudedAuthorize]
    [HttpGet("by_identity/{identity}")]
    [SuccessCloudedResponse<MachineDictionary>]
    public IActionResult GetByIdentity(
        [Required] [FromRoute] string identity,
        [Optional] [FromQuery] bool withRoles,
        [Optional] [FromQuery] bool withPermissions,
        [Optional] [FromQuery] bool withDomains
    )
    {
        var machine = machineService.GetByIdentity(
            identity!,
            withRoles,
            withPermissions,
            withDomains
        );
        return Ok(machine);
    }

    /// <summary>
    /// Get entity by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="withRoles"></param>
    /// <param name="withPermissions"></param>
    /// <param name="withDomains"></param>
    /// <returns><see cref="MachineDictionary"/></returns>
    [CloudedAuthorize]
    [HttpGet("{id}")]
    [SuccessCloudedResponse<MachineDictionary>]
    public virtual IActionResult GetById(
        [Required] [FromRoute] string id,
        [Optional] [FromQuery] bool withRoles,
        [Optional] [FromQuery] bool withPermissions,
        [Optional] [FromQuery] bool withDomains
    )
    {
        var entity = machineService.GetById(id, withRoles, withPermissions, withDomains);

        if (!entity.Any())
            throw new NotFoundException(EntityName);

        return Ok(entity);
    }

    /// <summary>
    /// Create entity
    /// </summary>
    /// <param name="data"></param>
    /// <returns><see cref="MachineDictionary"/></returns>
    [CloudedAuthorize]
    [HttpPost]
    [SuccessCloudedResponse<MachineDictionary>(StatusCodes.Status201Created)]
    public virtual IActionResult Create([Required] [FromBody] MachineDictionary data)
    {
        var entity = machineService.Create(data);

        return Created(Url.Action("GetById", EntityName, new { id = entity.Id }), entity);
    }

    /// <summary>
    /// Update entity
    /// </summary>
    /// <param name="id"></param>
    /// <param name="data"></param>
    /// <returns><see cref="MachineDictionary"/></returns>
    [CloudedAuthorize]
    [HttpPatch("{id}")]
    [SuccessCloudedResponse<MachineDictionary>]
    public virtual IActionResult Update(
        [Required] [FromRoute] string id,
        [Required] [FromBody] MachineDictionary data
    )
    {
        var entity = machineService.Update(id, data);

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
        machineService.DeleteById(id);
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
        [Required] [FromBody] MachineRoleInput input
    )
    {
        machineService.AddRoles(id, input);

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
        [Required] [FromBody] MachineRoleInput input
    )
    {
        machineService.RemoveRoles(id, input);

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
        [Required] [FromBody] MachinePermissionInput input
    )
    {
        machineService.AddPermissions(id, input);

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
        [Required] [FromBody] MachinePermissionInput input
    )
    {
        machineService.RemovePermissions(id, input);

        return Ok();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [CloudedAuthorize]
    [HttpPut("{id}/domains/add")]
    public IActionResult AddDomains(
        [Required] [FromRoute] string id,
        [Required] [FromBody] MachineDomainInput input
    )
    {
        machineService.AddDomains(id, input);

        return Ok();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [CloudedAuthorize]
    [HttpPut("{id}/domains/remove")]
    public IActionResult RemoveDomains(
        [Required] [FromRoute] string id,
        [Required] [FromBody] MachineDomainInput input
    )
    {
        machineService.RemoveDomains(id, input);

        return Ok();
    }
}
