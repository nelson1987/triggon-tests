//using System.Linq.Expressions;
//using MongoDB.Driver;
//using Triggon.Core.Contexts;
//using Triggon.Core.Entities;
//using Triggon.Core.Repositories;

//namespace Triggon.Core;
//public class SolicitacaoRepository : IMongoRepository<Solicitacao>
//{
//    private readonly IMyApplicationContext _context;

//    public SolicitacaoRepository(IMyApplicationContext context)
//    {
//        _context = context;
//    }

//    public async Task UpdateAsync(Solicitacao entity, CancellationToken cancellationToken = default)
//    {
//        throw new NotImplementedException();
//    }

//    public async Task InsertAsync(Solicitacao entity, CancellationToken cancellationToken = default)
//    {
//        await _context.Solicitations.InsertOneAsync(entity, cancellationToken);
//    }

//    public async Task<Solicitacao?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
//    {
//        return await _context.Solicitations.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
//    }

//    public Task<IEnumerable<Solicitacao>> FindByFilterAsync(Expression<Func<Solicitacao, bool>> filterExpression, CancellationToken cancellationToken = default)
//    {
//        throw new NotImplementedException();
//    }
//}