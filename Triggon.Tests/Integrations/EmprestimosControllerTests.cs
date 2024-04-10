using AutoFixture;
using AutoFixture.AutoMoq;
using MongoDB.Driver;
using System.Text;
using Triggon.Core.Entities;
using Triggon.Tests.Configs;

namespace Triggon.Tests.Integrations;

public class EmprestimosControllerTests : Configs.IntegrationTests, IClassFixture<TigerApiFixture>
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly TigerApiFixture _tigerApiFixture;
    private readonly Solicitacao _solicitacao;

    public EmprestimosControllerTests(TigerApiFixture tigerApiFixture)
    {
        _tigerApiFixture = tigerApiFixture;
        _solicitacao = _fixture.Build<Solicitacao>()
            .Create();
    }

    [Fact]
    public async Task Given_a_valid_request_on_update_rav_appetite_index()
    {
        // Arrange
        await KafkaFixture.ConsumeTopicsMessages();
        //Solicitacao _solicitacao = _fixture.Build<Solicitacao>()
        //    .Create();
        //await MongoDbFixture
        //    .Collection<Solicitacao>("warehouseTests", "Solicitacao")
        //    .InsertOneAsync(_solicitacao, CancellationToken.None);

        // Act
        var updateFinancialDataAsString = System.Text.Json.JsonSerializer.Serialize(_solicitacao);
        using var stringContent = new StringContent(updateFinancialDataAsString, Encoding.UTF8, "application/json");
        await _tigerApiFixture.Client.PostAsync("/Emprestimos", stringContent);
        //await _assetsManagerApiFixture.Client.PutAsJsonAsync("/investment-funds/rav/indexes", request);

        //Assert
        var todoCollection = await MongoDbFixture
            .Collection<Solicitacao>("Solicitacao")
            .FindAsync(x => x.Conta.Numero == _solicitacao.Conta.Numero);
        Solicitacao todoPersistido = todoCollection.FirstOrDefault();
        Assert.NotNull(todoPersistido);
        Assert.Equal(todoPersistido.Conta.Numero, _solicitacao.Conta.Numero);
        Assert.Equal(todoPersistido.Valor, _solicitacao.Valor);
        //Assert.Equal(todoPersistido.Criacao, _solicitacao.Criacao);
        Assert.Equal(todoPersistido.Tipo, _solicitacao.Tipo);
        Assert.NotNull(_solicitacao.Description);
    }
}