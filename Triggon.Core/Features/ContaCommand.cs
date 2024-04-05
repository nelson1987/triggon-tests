using Triggon.Core.Entities;

namespace Triggon.Core.Features;

public record ContaCommand(string Numero)
{
    public Conta ToEntity()
    {
        return new Conta()
        {
            Numero = Numero
        };
    }
}