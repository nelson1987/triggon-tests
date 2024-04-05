namespace Triggon.Core.Handlers;
public class InclusaoContaCorrenteHandler
{
    private readonly IGenericRepository<Conta> _repository;
    private readonly IGenericProducer<ContaCriadaEvent> _producer;

    public async Task<ContaCommandResponse> Handle(ContaCommand request, CancellationToken cancellationToken = default)
    {
        Conta entity = request.ToEntity();
        await _repository.Insert(entity, cancellationToken);

        ContaCriadaEvent @event = entity.ToEvent();
        await _producer.Send(@event, cancellationToken);

        return new ContaCommandResponse();
    }
}

public class Conta
{
    public ContaCriadaEvent ToEvent()
    {
        return new ContaCriadaEvent();
    }
}

public record ContaCommand
{
    public Conta ToEntity()
    {
        return new Conta();
    }
}

public record ContaCriadaEvent
{
}

public record ContaCommandResponse
{
}

public interface IGenericConsumer<T> where T : class
{
    Task<T> Consume(CancellationToken cancellationToken = default);
}

public interface IContaConsumer : IGenericConsumer<Conta>
{
}

public interface IGenericProducer<T> where T : class
{
    Task Send(T entity, CancellationToken cancellationToken = default);
}
public interface IContaProducer : IGenericProducer<Conta>
{
}

public interface IGenericRepository<T> where T : class
{
    Task Insert(T entity, CancellationToken cancellationToken = default);
}
public interface IContaRepository : IGenericRepository<Conta>
{
}

public interface IGenericHttpClient<T> where T : class
{
}
public interface IContaHttpClient : IGenericHttpClient<Conta>
{
}