using Triggon.Core.Entities;
using Triggon.Core.Features;

namespace Triggon.Core.Services;

public interface IContratoApi
{
    Task<bool> Autorizar(Conta conta, CancellationToken cancellationToken = default);
}

public interface IControladoriaApi
{
    Task Salvar(SolicitacaoCommand solicitacaoCommand, CancellationToken cancellationToken = default);
}

public interface ITesourariaApi
{
    Task Movimentar(SolicitacaoCommand solicitacaoCommand, CancellationToken cancellationToken = default);
}