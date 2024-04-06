using System.Linq.Expressions;
using MongoDB.Driver;
using Triggon.Core.Entities;
using Triggon.Core.Entities.Bases;
using Triggon.Core.Repositories;

namespace Triggon.Core.Contexts;
public class Usuario : EntityBase
{
    public string Name { get; set; }
}

public class MyApplicationContextOptions
{
    public required string ConnectionString { get; set; }
    public required string DatabaseName { get; set; }
}

public interface IMyApplicationContext
{
    IMongoCollection<Usuario> Users { get; }
    IMongoCollection<Solicitacao> Solicitations { get; }
    IMongoDatabase GetGenericDatabase();
}

public class MyApplicationContext : IMyApplicationContext
{
    private readonly IMongoDatabase _database;

    public MyApplicationContext(MyApplicationContextOptions options)
    {

        _database = new MongoClient(options.ConnectionString)
            .GetDatabase(options.DatabaseName);
    }

    public IMongoCollection<Usuario> Users => _database.GetCollection<Usuario>("Usuarios");
    public IMongoCollection<Solicitacao> Solicitations => _database.GetCollection<Solicitacao>("Solicitacoes");
    public IMongoDatabase GetGenericDatabase()
    {
        return _database;
    }
}

public class UserRepository : IMongoRepository<Usuario>
{
    private readonly IMyApplicationContext _context;

    public UserRepository(IMyApplicationContext context)
    {
        _context = context;
    }

    public async Task InsertAsync(Usuario entity, CancellationToken cancellationToken = default)
    {
        await _context.Users.InsertOneAsync(entity, cancellationToken);
    }

    public async Task<Usuario?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Usuario>> FindByFilterAsync(Expression<Func<Usuario, bool>> filterExpression, CancellationToken cancellationToken = default)
    {
        return await _context.Users.Find(filterExpression).ToListAsync(cancellationToken);
    }

    public Task UpdateAsync(Usuario entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}