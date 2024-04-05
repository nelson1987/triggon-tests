using AutoFixture;
using AutoFixture.AutoMoq;
using MongoDB.Driver;
using Triggon.Core;
using Triggon.Core.Entities;
using Triggon.Tests.Configs;

namespace Triggon.Tests.Integrations;
public class RepositoryIntegrationTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly CancellationToken _token = CancellationToken.None;
    private readonly Solicitacao _todo;
    private readonly SolicitacaoRepository _sut;

    public RepositoryIntegrationTests()
    {
        _todo = _fixture.Build<Solicitacao>()
            .Create();
        _sut = new SolicitacaoRepository(MongoDbFixture.Context());
    }

    [Fact]
    public async Task GivenTodoValid_InsertInto_ReturnSameEntity()
    {
        //Arrange
        var idTodo = _todo.Id;
        await MongoDbFixture.Collection<Solicitacao>("warehouseTests", "Solicitacao")
                            .InsertOneAsync(_todo, _token);
        var todoPersistido = await _sut.FindByIdAsync(idTodo);
        Assert.NotNull(todoPersistido);
        Assert.Equal(todoPersistido.Id, _todo.Id);
        Assert.Equal(todoPersistido.Description, _todo.Description);
    }

    [Fact]
    public async Task GivenTwoTodosValid_InsertInto_Return_DuplicateKeyException()
    {
        //arrange
        await MongoDbFixture.Collection<Solicitacao>("warehouseTests", "Solicitacao")
            .InsertOneAsync(_todo, _token);
        //act
        Func<Task> act = () => _sut.InsertAsync(_todo, _token);
        //assert
        MongoWriteException exception = await Assert.ThrowsAsync<MongoWriteException>(act);
        Assert.True(exception.Message.Contains("DuplicateKey"));
    }
}