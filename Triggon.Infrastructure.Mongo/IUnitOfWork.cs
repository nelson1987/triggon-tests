﻿using Triggon.Core.Entities;

namespace Triggon.Infrastructure.Mongo;

public interface IUnitOfWork : IDisposable
{
    GenericRepository<Solicitacao> DepartmentRepository { get; }
    GenericRepository<Customer> CourseRepository { get; }
    Task SaveAsync();
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