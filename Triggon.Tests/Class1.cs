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

public static class Producer
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
public interface IGenericConsumer<TEvent> where TEvent : AuditableEvent
{
    Task Subscribe();
}
public record MatriculaDto : AuditableDto { }
public interface IGenericHttpClient<TDto> where TDto : AuditableDto
{
    Task<TDto> GetAsync();
}