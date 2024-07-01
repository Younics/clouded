using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Clouded.Auth.Provider.Controllers.Private.Base;
using Clouded.Auth.Provider.Dictionaries;
using Clouded.Auth.Provider.Security;
using Clouded.Auth.Provider.Services.Interfaces;
using Clouded.Auth.Shared;
using Clouded.Auth.Shared.Domain;
using Clouded.Auth.Shared.Pagination;
using Clouded.Results;
using Clouded.Results.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Clouded.Auth.Provider.Controllers.Private;

[ApiController]
[Route($"v1/{RoutesConfig.DomainsRoutePrefix}")]
public class DomainController(IDomainService domainService) : BaseController
{
    private const string EntityName = "Domain";

    /// <summary>
    ///
    /// </summary>
    /// <param name="search"></param>
    /// <returns><see cref="IEnumerable{DomainDictionary}"/></returns>
    [CloudedAuthorize]
    [HttpGet]
    [SuccessCloudedResponse<IEnumerable<DomainDictionary>>]
    public IActionResult GetAll([FromQuery] string? search)
    {
        return Ok(domainService.GetAll(search));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="search"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <returns><see cref="Paginated{DomainDictionary}"/></returns>
    [CloudedAuthorize]
    [HttpGet("paginated")]
    [SuccessCloudedResponse<Paginated<DomainDictionary>>]
    public IActionResult GetAllPaginated(
        [FromQuery] string? search,
        [FromQuery] int page = 0,
        [FromQuery] int size = 50
    )
    {
        return Ok(domainService.GetAllPaginated(search, page, size));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="withMachines"></param>
    /// <returns><see cref="DomainDictionary"/></returns>
    [CloudedAuthorize]
    [HttpGet("by_identity/{identity}")]
    [SuccessCloudedResponse<DomainDictionary>]
    public IActionResult GetByIdentity(
        [Required] [FromRoute] string identity,
        [Optional] [FromQuery] bool withMachines
    )
    {
        var domain = domainService.GetByIdentity(identity!, withMachines);
        return Ok(domain);
    }

    /// <summary>
    /// Get entity by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="withMachines"></param>
    /// <returns><see cref="DomainDictionary"/></returns>
    [CloudedAuthorize]
    [HttpGet("{id}")]
    [SuccessCloudedResponse<DomainDictionary>]
    public virtual IActionResult GetById(
        [Required] [FromRoute] string id,
        [Optional] [FromQuery] bool withMachines
    )
    {
        var entity = domainService.GetById(id, withMachines);

        if (!entity.Any())
            throw new NotFoundException(EntityName);

        return Ok(entity);
    }

    /// <summary>
    /// Create entity
    /// </summary>
    /// <param name="data"></param>
    /// <returns><see cref="DomainDictionary"/></returns>
    [CloudedAuthorize]
    [HttpPost]
    [SuccessCloudedResponse<DomainDictionary>(StatusCodes.Status201Created)]
    public virtual IActionResult Create([Required] [FromBody] DomainDictionary data)
    {
        var entity = domainService.Create(data);

        return Created(Url.Action("GetById", EntityName, new { id = entity.Id }), entity);
    }

    /// <summary>
    /// Update entity
    /// </summary>
    /// <param name="id"></param>
    /// <param name="data"></param>
    /// <returns><see cref="DomainDictionary"/></returns>
    [CloudedAuthorize]
    [HttpPatch("{id}")]
    [SuccessCloudedResponse<DomainDictionary>]
    public virtual IActionResult Update(
        [Required] [FromRoute] string id,
        [Required] [FromBody] DomainDictionary data
    )
    {
        var entity = domainService.Update(id, data);

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
        domainService.DeleteById(id);
        return Ok();
    }

    /// <summary>
    /// Attach machine to domain
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [CloudedAuthorize]
    [HttpPut("{id}/machines/add")]
    public IActionResult AddMachines(
        [Required] [FromRoute] string id,
        [Required] [FromBody] DomainMachineInput input
    )
    {
        domainService.AddMachines(id, input);

        return Ok();
    }

    /// <summary>
    /// Detach machine from domain
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [CloudedAuthorize]
    [HttpPut("{id}/machines/remove")]
    public IActionResult RemoveMachines(
        [Required] [FromRoute] string id,
        [Required] [FromBody] DomainMachineInput input
    )
    {
        domainService.RemoveMachines(id, input);

        return Ok();
    }
}
