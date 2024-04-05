using MongoDB.Driver;
using Triggon.Core.Entities;
using Triggon.Core.Entities.Bases;
using Triggon.Core.Repositories;

namespace Triggon.Core;
public abstract class MongoRepository<T> : IMongoRepository<T> where T : EntityBase
{
    public IMongoCollection<T> Collection { get; }

    public MongoRepository(IMongoContext mongoDbContext, string collection)
    {
        Collection = mongoDbContext.Database.GetCollection<T>(collection);
    }

    public async Task InsertAsync(T entity, CancellationToken cancellationToken = default)
    {
        await Collection.InsertOneAsync(entity, cancellationToken);
    }

    public async Task<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

public class SolicitacaoRepository : MongoRepository<Solicitacao>, ISolicitacaoRepository
{
    public SolicitacaoRepository(IMongoContext mongoDbContext) : base(mongoDbContext, "Solicitacao")
    {
    }
}

public class MongoContext : IMongoContext
{
    public IMongoDatabase Database { get; }
    public MongoContext()
    {
        var connectionString = "mongodb://root:password@localhost:27017/";
        MongoClient Context = new MongoClient(connectionString);
        Database = Context
            .GetDatabase("warehouseTests");
    }
}