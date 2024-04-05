using Triggon.Core.Entities;

namespace Triggon.Core.Features;

public record SolicitacaoCommand(ContaCommand Conta, decimal Valor, DateTime Criacao, TipoSolicitacao Tipo)
{
    public Solicitacao ToEntity()
    {
        return new Solicitacao()
        {
            Valor = Valor,
            Criacao = Criacao,
            Tipo = Tipo,
            Conta = Conta.ToEntity()
        };
    }
}
