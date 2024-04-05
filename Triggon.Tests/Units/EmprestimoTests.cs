using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using Triggon.Core;
using Triggon.Core.Entities;
using Triggon.Core.Features;
using Triggon.Core.Services;

namespace Triggon.Tests.Units;

/// <summary>
/// eu tenho um site comercial
/// cliente cria demandas
/// pede empréstimo
/// eu tenho uma api de autorização mantida pela área de contratos
/// confirma/rejeita a criação dos contratos
/// eu tenho uma api de Controladoria
/// persiste todos contratos
/// eu tenho uma api de tesouraria
/// realiza transferência de valores para conta dos clientes
/// </summary>
public class EmprestimoTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly Mock<IContratoApi> _mockContratoApi;
    private readonly Mock<IControladoriaApi> _mockControladoriaApi;
    private readonly Mock<ITesourariaApi> _mockTesourariaApi;
    private readonly CancellationToken token = CancellationToken.None;
    private readonly SolicitacaoCommand _solicitacao;
    private readonly Emprestimo _sut;

    public EmprestimoTests()
    {
        _solicitacao = _fixture.Build<SolicitacaoCommand>()
            .With(x => x.Tipo, TipoSolicitacao.Emprestimo)
            .Create();

        _mockContratoApi = _fixture.Freeze<Mock<IContratoApi>>();
        _mockControladoriaApi = _fixture.Freeze<Mock<IControladoriaApi>>();
        _mockTesourariaApi = _fixture.Freeze<Mock<ITesourariaApi>>();

        _mockContratoApi.Setup(x => x.Autorizar(It.IsAny<Conta>(), token))
            .ReturnsAsync(true);
        _mockControladoriaApi.Setup(x => x.Salvar(It.IsAny<SolicitacaoCommand>(), token))
            .Returns(Task.CompletedTask);
        _mockTesourariaApi.Setup(x => x.Movimentar(It.IsAny<SolicitacaoCommand>(), token))
            .Returns(Task.CompletedTask);

        _sut = _fixture.Create<Emprestimo>();
    }

    [Fact]
    public async Task DadoNumeroContaValida_EmiteEmprestimo_AumentandoSaldo_ComSucesso()
    {
        //act
        await _sut.Solicitar(_solicitacao, token);
        _mockContratoApi.Verify(x => x.Autorizar(It.IsNotNull<Conta>(), token), Times.Once);
        _mockControladoriaApi.Verify(x => x.Salvar(It.IsNotNull<SolicitacaoCommand>(), token), Times.Once);
        _mockTesourariaApi.Verify(x => x.Movimentar(It.IsNotNull<SolicitacaoCommand>(), token), Times.Once);
    }

    [Fact]
    public async Task DadoTipoSolicitacaoInValida_RetornaErro()
    {
        //arrange
        var solicitacao = _solicitacao with { Tipo = TipoSolicitacao.Cartao };
        //act
        Func<Task> act = () => _sut.Solicitar(solicitacao, token);
        //assert
        TriggonException exception = await Assert.ThrowsAsync<TriggonException>(act);
        Assert.Equal("Tipo Inválido", exception.Message);
    }

    [Fact]
    public async Task DadoNumeroContaInValida_RetornaErro()
    {
        //arrange
        var solicitacao = _solicitacao with { Conta = default };
        //act
        Func<Task> act = () => _sut.Solicitar(solicitacao, token);
        //assert
        TriggonException exception = await Assert.ThrowsAsync<TriggonException>(act);
        Assert.Equal("Conta Inválida", exception.Message);
    }

    [Fact]
    public async Task DadoValorEmprestimoInValido_RetornaErro()
    {
        //arrange
        var solicitacao = _solicitacao with { Valor = 0.00M };
        //act
        Func<Task> act = () => _sut.Solicitar(solicitacao, token);
        //assert
        TriggonException exception = await Assert.ThrowsAsync<TriggonException>(act);
        Assert.Equal("Valor Inválido", exception.Message);
    }

    [Fact]
    public async Task DadoContratoApiRejeitado_RetornaErro()
    {
        //Arrange
        _fixture.Freeze<Mock<IContratoApi>>()
            .Setup(x => x.Autorizar(It.IsAny<Conta>(), token))
            .ReturnsAsync(false);
        //act
        Func<Task> act = () => _sut.Solicitar(_solicitacao, token);
        //assert
        TriggonException exception = await Assert.ThrowsAsync<TriggonException>(act);
        Assert.Equal("Não Autorizado", exception.Message);
        _mockContratoApi.Verify(x => x.Autorizar(It.IsNotNull<Conta>(), token), Times.Once);
        _mockControladoriaApi.Verify(x => x.Salvar(It.IsNotNull<SolicitacaoCommand>(), token), Times.Never);
        _mockTesourariaApi.Verify(x => x.Movimentar(It.IsNotNull<SolicitacaoCommand>(), token), Times.Never);
    }

    [Fact]
    public async Task DadoControladoriaApiFalha_RetornaException()
    {
        //Arrange
        _fixture.Freeze<Mock<IControladoriaApi>>()
            .Setup(x => x.Salvar(It.IsAny<SolicitacaoCommand>(), token))
            .Throws(new TriggonException("Falha em Controladoria Api"));
        //act
        Func<Task> act = () => _sut.Solicitar(_solicitacao, token);
        //assert
        TriggonException exception = await Assert.ThrowsAsync<TriggonException>(act);
        Assert.Equal("Falha em Controladoria Api", exception.Message);
        _mockContratoApi.Verify(x => x.Autorizar(It.IsNotNull<Conta>(), token), Times.Once);
        _mockControladoriaApi.Verify(x => x.Salvar(It.IsNotNull<SolicitacaoCommand>(), token), Times.Once);
        _mockTesourariaApi.Verify(x => x.Movimentar(It.IsNotNull<SolicitacaoCommand>(), token), Times.Never);

    }

    [Fact]
    public async Task DadoTesourariaApiFalha_RetornaException()
    {
        //Arrange
        _fixture.Freeze<Mock<ITesourariaApi>>()
            .Setup(x => x.Movimentar(It.IsAny<SolicitacaoCommand>(), token))
            .Throws(new TriggonException("Falha em Tesouraria Api"));
        //act
        Func<Task> act = () => _sut.Solicitar(_solicitacao, token);
        //assert
        TriggonException exception = await Assert.ThrowsAsync<TriggonException>(act);
        Assert.Equal("Falha em Tesouraria Api", exception.Message);
        _mockContratoApi.Verify(x => x.Autorizar(It.IsNotNull<Conta>(), token), Times.Once);
        _mockControladoriaApi.Verify(x => x.Salvar(It.IsNotNull<SolicitacaoCommand>(), token), Times.Once);
        _mockTesourariaApi.Verify(x => x.Movimentar(It.IsNotNull<SolicitacaoCommand>(), token), Times.Once);
    }
}
