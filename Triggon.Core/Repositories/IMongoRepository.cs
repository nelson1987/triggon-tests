using MongoDB.Driver;
using Triggon.Core.Entities;
using Triggon.Core.Entities.Bases;

namespace Triggon.Core.Repositories;

public interface IMongoRepository<T> where T : EntityBase
{
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task InsertAsync(T entity, CancellationToken cancellationToken = default);
    Task<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ISolicitacaoRepository : IMongoRepository<Solicitacao>
{
}

public interface IMongoContext
{
    IMongoDatabase Database { get; }
}
