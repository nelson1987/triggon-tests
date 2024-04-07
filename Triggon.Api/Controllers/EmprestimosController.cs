using Microsoft.AspNetCore.Mvc;
using Triggon.Core.Entities;
using Triggon.Core.Features;
using Triggon.Infrastructure.Mongo;

namespace Triggon.Api.Controllers;
[ApiController]
[Route("[controller]")]
public class EmprestimosController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Post([FromServices] IGenericRepository<Solicitacao> repository,
        [FromBody] SolicitacaoCommand solicitacao
        , CancellationToken cancellationToken)
    {
        await repository.InsertAsync(solicitacao.ToEntity(), cancellationToken);
        return Ok();
    }

    [HttpGet("/teste")]
    public async Task<IActionResult> SomeAction([FromServices] IGenericRepository<Customer> _repository)//, CancellationToken cancellationToken)
    {
        return Ok(await _repository.GetAll());
    }

    [HttpPost("/teste")]
    public async Task<IActionResult> SomeActionPost([FromServices] IGenericRepository<Customer> unitOfWork, CancellationToken cancellationToken)
    {
        await unitOfWork.InsertAsync(new Customer() { Name = "Name", Order = "Order" }, cancellationToken);
        //SolicitacaoCommand solicitacao = new SolicitacaoCommand(new ContaCommand("Numero"),10.00M, DateTime.Now, TipoSolicitacao.Emprestimo);
        //await unitOfWork.DepartmentRepository.InsertAsync(solicitacao.ToEntity(), cancellationToken);
        //await unitOfWork.SaveAsync();
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