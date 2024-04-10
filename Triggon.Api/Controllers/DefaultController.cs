using System.Text;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RabbitMQ.Client;
using Triggon.Core.Features;

namespace Triggon.Api.Controllers;
//https://www.mongodb.com/developer/languages/csharp/transactions-csharp-dotnet/
[ApiController]
[Route("[controller]")]
public class CreditosController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CreditoCommand solicitacao, CancellationToken cancellationToken)
    {
        string MongoDBConnectionString = "mongodb://root:password@localhost:27017/";
        var client = new MongoClient(MongoDBConnectionString);
        var session = client.StartSession();
        var products = session.Client.GetDatabase("MongoDBStore")
            .GetCollection<Product>("products");
        products.Database.DropCollection("products");
        var TV = new Product { Description = "Television", SKU = 4001, Price = 2000 };
        await products.InsertOneAsync(TV);


        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "orders",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var message = "Hello World!";
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "", 
            routingKey: "orders",
            basicProperties: null, 
            body: body);

        return Ok();
    }
}

public class Product
{
    public Guid Id { get; set; }
    public decimal Price { get; set; }
    public int SKU { get; set; }
    public string Description { get; set; }
}

public class ContaBancaria
{
    public Guid Id { get; set; }
    public string Numero { get; set; }
}

//public static class CreditoDependencies
//{
//    public static IServiceCollection AddCredito(this IServiceCollection services)
//    {
//        services.AddScoped<IContextRepository, ContextRepository>();
//        services.AddScoped<IUnitOfWork, UnitOfWork>();
//        services.AddScoped<ICreditoRepository, CreditoRepository>();
//        services.AddScoped<IContaRepository, ContaRepository>();
//        return services;
//    }
//}

public record CreditoCommand(ContaCommand Conta, decimal Valor, DateTime Criacao);
/*
public interface ICreditoRepository
{
    Task Incluir(Product credito);
}

public class CreditoRepository : ICreditoRepository
{
    private readonly IContextRepository _contextRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMongoCollection<Product> _contaCollection;

    public CreditoRepository(IContextRepository contextRepository, IUnitOfWork unitOfWork)
    {
        _contextRepository = contextRepository;
        _unitOfWork = unitOfWork;
        _contaCollection = contextRepository.getDatabase().GetCollection<Product>("credito");
    }

    public Task Incluir(Product credito)
    {
        Action operation = () => _contaCollection.InsertOne(_unitOfWork.Session as IClientSessionHandle, credito);
        _unitOfWork.AddOperation(operation);
        return Task.CompletedTask;
    }
}

public interface IContaRepository
{
    Task Update(ContaBancaria conta);
}

public class ContaRepository : IContaRepository
{
    private readonly IContextRepository _contextRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMongoCollection<ContaBancaria> _contaCollection;

    public ContaRepository(IContextRepository contextRepository, IUnitOfWork unitOfWork)
    {
        _contextRepository = contextRepository;
        _unitOfWork = unitOfWork;
        _contaCollection = contextRepository.getDatabase().GetCollection<ContaBancaria>("contas");
    }

    public Task Update(ContaBancaria conta)
    {
        Action operation = () => _contaCollection.ReplaceOne(_unitOfWork.Session as IClientSessionHandle, 
            x => x.Id == conta.Id, conta);
        _unitOfWork.AddOperation(operation);
        //throw new NotImplementedException();
        return Task.CompletedTask;
    }
}

//public interface IUnitOfWork
//{
//    IDisposable Session { get; }
//    void BeginTransaction();
//    void AddOperation(Action operation);
//    Task Commit();
//    void Rollback();
//}

//public class UnitOfWork : IUnitOfWork
//{
//    private IClientSessionHandle session { get; }
//    public IDisposable Session => this.session;
//    private List<Action> _operations { get; set; }

//    private readonly IContextRepository _contextRepository;
//    public UnitOfWork(IContextRepository contextRepository)
//    {
//        _contextRepository = contextRepository;
//        this.session = new MongoClient(contextRepository.ConnectionString).StartSession();
//        this._operations = new List<Action>();
//    }

//    public void BeginTransaction()
//    {
//        this.session.StartTransaction();
//    }

//    public void AddOperation(Action operation)
//    {
//        this._operations.Add(operation);
//    }

//    public async Task Commit()
//    {
//        this._operations.ForEach(o =>
//        {
//            o.Invoke();
//        });

//        await this.session.CommitTransactionAsync();
//    }

//    public void Rollback()
//    {
//        this._operations.Clear();
//    }
//}

public interface IContextRepository
{
    string ConnectionString { get; set; }
    string DatabaseName { get; set; }
    IMongoDatabase getDatabase();
}

public class ContextRepository : IContextRepository
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public IMongoDatabase getDatabase()
    {
        var mongoClient = new MongoClient(ConnectionString);
        return mongoClient.GetDatabase(DatabaseName);
    }
}
*/