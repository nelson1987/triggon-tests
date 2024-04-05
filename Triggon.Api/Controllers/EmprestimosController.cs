using Microsoft.AspNetCore.Mvc;
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
}