using AutoFixture;
using AutoFixture.AutoMoq;
using FluentValidation;
using FluentValidation.TestHelper;
using Triggon.Core;
using Triggon.Core.Entities;
using Triggon.Core.Features;

namespace Triggon.Tests.Units;

public class SolicitacaoValidatorTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly IValidator<SolicitacaoCommand> _validator;
    private readonly SolicitacaoCommand _solicitacaoCommand;

    public SolicitacaoValidatorTests()
    {
        _solicitacaoCommand = _fixture.Build<SolicitacaoCommand>()
            .With(x => x.Tipo, TipoSolicitacao.Emprestimo)
            .Create();
        _validator = _fixture.Create<SolicitacaoValidator>();
    }

    [Fact]
    public async Task DadoSolicitacaoValida_RetornaFalha()
        => _validator
            .TestValidate(_solicitacaoCommand)
            .ShouldNotHaveAnyValidationErrors();

    [Fact]
    public async Task DadoSolicitacaoInvalida_ContaVazia_RetornaFalha()
        => _validator
            .TestValidate(_solicitacaoCommand with { Conta = default })
            .ShouldHaveValidationErrorFor(x => x.Conta)
            .Only();

    [Fact]
    public async Task DadoSolicitacaoInvalida_ValorVazio_RetornaFalha()
        => _validator
            .TestValidate(_solicitacaoCommand with { Valor = default })
            .ShouldHaveValidationErrorFor(x => x.Valor)
            .Only();

    [Fact]
    public async Task DadoSolicitacaoInvalida_ValorZero_RetornaFalha()
        => _validator
            .TestValidate(_solicitacaoCommand with { Valor = 0.00M })
            .ShouldHaveValidationErrorFor(x => x.Valor)
            .Only();

    [Fact]
    public async Task DadoSolicitacaoInvalida_CriacaoVazio_RetornaFalha()
        => _validator
            .TestValidate(_solicitacaoCommand with { Criacao = default })
            .ShouldHaveValidationErrorFor(x => x.Criacao)
            .Only();

    [Fact]
    public async Task DadoSolicitacaoInvalida_TipoInvalido_RetornaFalha()
        => _validator
            .TestValidate(_solicitacaoCommand with { Tipo = (TipoSolicitacao)999 })
            .ShouldHaveValidationErrorFor(x => x.Tipo)
            .Only();
}
