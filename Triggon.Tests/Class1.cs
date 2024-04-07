using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Triggon.Tests;
public class RabbitMqOptions
{
    public required string ConnectionString { get; set; }
}
public class MongoDbOptions
{
    public required string ConnectionString { get; set; }
    public required string Database { get; set; }
}

public static class Dependencies
{
    public static IServiceCollection AddContext(this IServiceCollection services)
    {
        services.Configure<RabbitMqOptions>(x =>
        {
            x.ConnectionString = "";
        });
        services.Configure<MongoDbOptions>(x =>
        {
            x.ConnectionString = "";
            x.Database = "";
        });
        services.AddScoped<IMongoGenericContext, MongoGenericContext>();
        services.AddScoped<IGenericRepository<Aluno>, AlunoGenericRepository>();
        services.AddScoped<IGenericRepository<Materia>, MateriaGenericRepository>();

        services.AddScoped<IGenericProducer<AlunoMatriculadoEvent>, MateriaGenericRepository>();

        return services;
    }
    /*
     
builder.Services.Configure<PositionOptions>(
    builder.Configuration.GetSection(PositionOptions.Position));
     
     public class Test2Model : PageModel
{
    private readonly PositionOptions _options;

    public Test2Model(IOptions<PositionOptions> options)
    {
        _options = options.Value;
    }

    public ContentResult OnGet()
    {
        return Content($"Title: {_options.Title} \n" +
                       $"Name: {_options.Name}");
    }
}
     
     */
}

public interface IMongoGenericContext
{
    IMongoDatabase GetDatabase();
}
public class MongoGenericContext : IMongoGenericContext
{
    private readonly MongoDbOptions _options;
    private readonly IMongoDatabase _database;
    //public IMongoCollection<Aluno> aluno => _database.GetCollection<Aluno>("Aluno");
    public MongoGenericContext(IOptions<MongoDbOptions> options)
    {
        _options = options.Value;
        var mongo = new MongoClient(_options.ConnectionString);
        _database = mongo.GetDatabase(_options.Database);
    }
    public IMongoDatabase GetDatabase()
    {
        return _database;
    }
}
public abstract class AuditableEntity { }
public abstract record AuditableEvent { }
public abstract record AuditableDto { }
public class Aluno : AuditableEntity { }
public class Materia : AuditableEntity { }
public interface IGenericRepository<TEntity> where TEntity : AuditableEntity
{
    Task CreateAsync(TEntity entity);
}
public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : AuditableEntity
{
    private readonly IMongoGenericContext _context;
    private readonly IMongoCollection<TEntity> _collection;

    protected GenericRepository(IMongoGenericContext context, string collectionName)
    {
        _context = context;
        _collection = _context.GetDatabase().GetCollection<TEntity>(collectionName);
    }

    public Task CreateAsync(TEntity entity)
    {
        _collection.InsertOne(entity);
        throw new NotImplementedException();
    }
}
public interface IAlunoGenericRepository : IGenericRepository<Aluno> { }
public class AlunoGenericRepository : GenericRepository<Aluno>, IAlunoGenericRepository
{
    public AlunoGenericRepository(IMongoGenericContext context) : base(context, "Alunos")
    {
    }
}
public interface IMateriaGenericRepository : IGenericRepository<Materia> { }
public class MateriaGenericRepository : GenericRepository<Materia>, IMateriaGenericRepository
{
    public MateriaGenericRepository(IMongoGenericContext context, string collectionName) : base(context, collectionName)
    {
    }
}
public interface IUnitOfWork
{
    IGenericRepository<Aluno> Alunos { get; }
    IGenericRepository<Materia> Materias { get; }
    Task SaveAsync();
}
public record AlunoMatriculadoEvent : AuditableEvent { }
public interface IGenericProducer<TEvent> where TEvent : AuditableEvent
{
    Task Publish(TEvent @event);
}
public abstract class GenericProducer<TEvent> : IGenericProducer<TEvent> where TEvent : AuditableEvent
{
    private readonly RabbitMqOptions _options;
    public GenericProducer(IOptions<RabbitMqOptions> options)
    {
        _options = options.Value;
    }
    public Task Publish(TEvent @event)
    {
        throw new NotImplementedException();
    }
}

public interface IAlunoMatriculadoProducer : IGenericProducer<AlunoMatriculadoEvent> { }
public class AlunoMatriculadoProducer : GenericProducer<AlunoMatriculadoEvent>, IAlunoMatriculadoProducer
{
    public AlunoMatriculadoProducer(IOptions<RabbitMqOptions> options) : base(options)
    {
    }
}

public interface IGenericConsumer<TEvent> where TEvent : AuditableEvent
{
    Task Subscribe();
}
public record MatriculaDto : AuditableDto { }
public interface IGenericHttpClient<TDto> where TDto : AuditableDto
{
    Task<TDto> GetAsync();
}