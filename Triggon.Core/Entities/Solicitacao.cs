using Triggon.Core.Entities.Bases;

namespace Triggon.Core.Entities;

public class Solicitacao : EntityBase
{
    public Conta Conta { get; set; }
    public decimal Valor { get; set; }
    public DateTime Criacao { get; set; }
    public TipoSolicitacao Tipo { get; set; }
    public string Description { get; set; }
}
