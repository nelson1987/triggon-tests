using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Triggon.Infrastructure.Mongo;

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