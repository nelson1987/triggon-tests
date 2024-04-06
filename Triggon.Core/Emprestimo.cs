using FluentValidation;
using Triggon.Core.Entities;
using Triggon.Core.Features;
using Triggon.Core.Repositories;
using Triggon.Core.Services;

namespace Triggon.Core;
public class Emprestimo
{
    private readonly IContratoApi _contratoApi;
    private readonly IControladoriaApi _controladoriaApi;
    private readonly ITesourariaApi _tesourariaApi;
    private readonly IMongoRepository<Solicitacao> _mongoRepository;
    public Emprestimo(IContratoApi contratoApi,
        IControladoriaApi controladoriaApi,
        ITesourariaApi tesourariaApi,
        IMongoRepository<Solicitacao> mongoRepository)
    {
        _contratoApi = contratoApi;
        _controladoriaApi = controladoriaApi;
        _tesourariaApi = tesourariaApi;
        _mongoRepository = mongoRepository;
    }

    public async Task Solicitar(SolicitacaoCommand solicitacaoCommand, CancellationToken cancellationToken = default)
    {
        if (solicitacaoCommand.Tipo != TipoSolicitacao.Emprestimo) throw new TriggonException("Tipo Inválido");
        if (solicitacaoCommand.Conta == null) throw new TriggonException("Conta Inválida");
        if (solicitacaoCommand.Valor <= 0) throw new TriggonException("Valor Inválido");
        if (!await _contratoApi.Autorizar(solicitacaoCommand.Conta.ToEntity(), cancellationToken)) throw new TriggonException("Não Autorizado");
        await _controladoriaApi.Salvar(solicitacaoCommand, cancellationToken);
        //var membro = await _mongoRepository.FindByFilterAsync(Guid.NewGuid());
        //membro.Description = "";
        //await _mongoRepository.UpdateAsync(membro);

        await _tesourariaApi.Movimentar(solicitacaoCommand, cancellationToken);
    }
}

public class SolicitacaoValidator : AbstractValidator<SolicitacaoCommand>
{
    public SolicitacaoValidator()
    {
        RuleFor(x => x.Conta).NotEmpty();
        RuleFor(x => x.Valor).NotEmpty().GreaterThan(0);
        RuleFor(x => x.Criacao).NotEmpty();
        RuleFor(x => x.Tipo).IsInEnum();
    }
}