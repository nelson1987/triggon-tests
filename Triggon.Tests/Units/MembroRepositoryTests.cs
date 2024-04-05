using AutoFixture.AutoMoq;
using AutoFixture;
using Triggon.Core;
using Triggon.Core.Entities;

namespace Triggon.Tests.Units;
public class RepositoryUnitTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly CancellationToken _token = CancellationToken.None;
    private readonly SolicitacaoRepository _sut;

    public RepositoryUnitTests()
    {
        _sut = _fixture.Build<SolicitacaoRepository>()
        .Create();
    }

    [Fact]
    public async Task GivenTodoValid_InsertInto()
    {
        await _sut.InsertAsync(new Solicitacao(), _token);
    }
}
