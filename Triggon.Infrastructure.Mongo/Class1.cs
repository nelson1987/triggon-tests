using Microsoft.EntityFrameworkCore;
using Triggon.Core.Entities;
using Triggon.Core.Entities.Bases;

namespace Triggon.Infrastructure.Mongo;

public class GenericRepository<T> : IGenericRepository<T>, IAsyncDisposable where T : MongoEntiyBase
{
    private readonly ApplicationDbContext _contextFactory;

    public GenericRepository(ApplicationDbContext contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<T>> GetAll()
    {
        //using var context = _contextFactory.CreateDbContext();
        return await _contextFactory.Set<T>().ToListAsync();
    }
    public async Task InsertAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _contextFactory.Set<T>().AddAsync(entity, cancellationToken);
        await _contextFactory.SaveChangesAsync(cancellationToken);
    }

    public async Task Save(CancellationToken cancellationToken = default)
    {
        await _contextFactory.SaveChangesAsync(cancellationToken);
    }
    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _contextFactory.Dispose();
            }
        }
        this.disposed = true;
    }
    protected virtual async Task DisposeAsync(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                await _contextFactory.DisposeAsync();
            }
        }
        this.disposed = true;
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }
}

//public interface IGenericRepositoryUsuario : IGenericRepository<Usuario>
//{
//}

//public class UserGenericRepository : GenericRepository<Usuario>, IGenericRepositoryUsuario
//{
//    public UserGenericRepository(ApplicationDbContext contextFactory) : base(contextFactory)
//    {
//    }
//}

public interface IGenericRepositorySolicitacao : IGenericRepository<Solicitacao>
{
}

public class SolicitacaoGenericRepository : GenericRepository<Solicitacao>, IGenericRepositorySolicitacao
{
    public SolicitacaoGenericRepository(ApplicationDbContext contextFactory) : base(contextFactory)
    {
    }
}
/*
public class UnitOfWork : IUnitOfWork
{
    private required ApplicationDbContext context;
    private required GenericRepository<Customer> genericRepository;
    private required GenericRepository<Solicitacao> courseRepository;
    public GenericRepository<Solicitacao> DepartmentRepository
    {
        get
        {

            if (this.courseRepository == null)
            {
                this.courseRepository = new GenericRepository<Solicitacao>(context);
            }
            return courseRepository;
        }
    }
    public GenericRepository<Customer> CourseRepository
    {
        get
        {

            if (this.genericRepository == null)
            {
                this.genericRepository = new GenericRepository<Customer>(context);
            }
            return genericRepository;
        }
    }
    public void Save()
    {
        context.SaveChanges();
    }
    public async Task SaveAsync()
    {
        await context.SaveChangesAsync();
    }
    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                context.Dispose();
            }
        }
        this.disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
*/