using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.EntityFrameworkCore.Extensions;
using System.Threading;
using Triggon.Core.Contexts;
using Triggon.Core.Entities;
using Triggon.Core.Entities.Bases;

namespace Triggon.Infrastructure.Mongo
{
    public static class MongoDependencies
    {
        public static void AddInsfrastructure(this IServiceCollection services)
        {

            services.AddDbContext<ApplicationDbContext>(opt =>
            {
                opt
                    .EnableSensitiveDataLogging()
                    .UseMongoDB("mongodb://root:password@localhost:27017/", "warehouseTests");
            });
            //services.AddScoped<IGenericRepository<Usuario>, UserGenericRepository>();
            services.AddScoped<IGenericRepository<Solicitacao>, SolicitacaoGenericRepository>();
            services.AddScoped<IGenericRepository<Customer>, CustomerGenericRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
    
    public class Customer : MongoEntiyBase
    {
        public  string Name { get; set; }
        public  string Order { get; set; }
    }

    public class ApplicationDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; init; }
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Customer>().ToCollection("Customers");
        }
    }

    public interface IGenericRepository<T> : IDisposable where T : class
    {
        Task<List<T>> GetAll();
        Task InsertAsync(T entity, CancellationToken cancellationToken = default);
        Task Save(CancellationToken cancellationToken = default);
    }

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
    public interface IGenericRepositoryCustomer : IGenericRepository<Customer>
    {
    }

    public class CustomerGenericRepository : GenericRepository<Customer>, IGenericRepositoryCustomer
    {
        public CustomerGenericRepository(ApplicationDbContext contextFactory) : base(contextFactory)
        {
        }
    }

    public interface IUnitOfWork : IDisposable
    {
        GenericRepository<Solicitacao> DepartmentRepository { get; }
        GenericRepository<Customer> CourseRepository { get; }
        Task SaveAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext context;
        private GenericRepository<Customer> genericRepository;
        private GenericRepository<Solicitacao> courseRepository;
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
}
