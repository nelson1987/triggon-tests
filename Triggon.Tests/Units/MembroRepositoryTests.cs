using AutoFixture;
using AutoFixture.AutoMoq;
using Triggon.Core.Entities;
using Triggon.Infrastructure.Mongo;

namespace Triggon.Tests.Units;
public class RepositoryUnitTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly CancellationToken _token = CancellationToken.None;
    private readonly SolicitacaoGenericRepository _sut;

    public RepositoryUnitTests()
    {
        _sut = _fixture.Build<SolicitacaoGenericRepository>()
        .Create();
    }

    [Fact]
    public async Task GivenTodoValid_InsertInto()
    {
        await _sut.InsertAsync(new Solicitacao(), _token);
    }
}
