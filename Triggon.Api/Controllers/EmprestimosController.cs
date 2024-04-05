using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Triggon.Core.Contexts;
using Triggon.Core.Entities;
using Triggon.Core.Features;
using Triggon.Core.Repositories;

namespace Triggon.Api.Controllers;
[ApiController]
[Route("[controller]")]
public class EmprestimosController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Post([FromServices] IMongoRepository<Solicitacao> repository,
        [FromBody] SolicitacaoCommand solicitacao
        , CancellationToken cancellationToken)
    {
        await repository.InsertAsync(solicitacao.ToEntity(), cancellationToken);
        return Ok();
    }

    [HttpGet("/teste")]
    public async Task<IActionResult> SomeAction([FromServices] IMyApplicationContext _context, CancellationToken cancellationToken)
    {
        return Ok(await _context.Users.Find(x => x.Id != Guid.Empty).ToListAsync(cancellationToken));
    }

    [HttpPost("/teste")]
    public async Task<IActionResult> SomeActionPost([FromServices] IMyApplicationContext _context, CancellationToken cancellationToken)
    {
        await _context.Users.InsertOneAsync(new Usuario() { Id = Guid.NewGuid() }, cancellationToken);
        return Created();
    }

    [HttpGet]
    public async Task<IActionResult> SomeAction([FromServices] IHttpClientFactory httpClientFactory)
    {
        // Get an HttpClient configured to the specification you defined in StartUp.
        var client = httpClientFactory.CreateClient("Github");

        return Ok(await client.GetStringAsync("/someapi"));
    }
}