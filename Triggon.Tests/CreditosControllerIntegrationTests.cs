using AutoFixture;
using AutoFixture.AutoMoq;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Triggon.Api.Controllers;
using Triggon.Tests.Configs;

namespace Triggon.Tests.IntegrationTests;
public static class MongoDbFixture
{
    public static List<Product> Produtos()
    {

        string MongoDBConnectionString = "mongodb://root:password@localhost:27017/";
        var client = new MongoClient(MongoDBConnectionString);
        var session = client.StartSession();
        var products = session.Client.GetDatabase("MongoDBStore").GetCollection<Product>("products");
        return products.Find(x => x.Id != Guid.Empty).ToList();
    }
}
public static class RabbitMqFixture
{
    public static List<string> Produtos()
    {
        List<string> consumeList = new List<string>();

        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "orders",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        //Set Event object which listen message from chanel which is sent by producer
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            consumeList.Add(message);
        };
        Thread.Sleep(TimeSpan.FromSeconds(5));
        channel.BasicConsume(queue: "orders", autoAck: true, consumer: consumer);
        Thread.Sleep(TimeSpan.FromSeconds(5));
        return consumeList;
    }

}

public class CreditosControllerIntegrationTests : Configs.IntegrationTests, IClassFixture<TigerApiFixture>
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly TigerApiFixture _tigerApiFixture;
    private CreditoCommand _command => _fixture.Build<CreditoCommand>().Create();

    public CreditosControllerIntegrationTests(TigerApiFixture tigerApiFixture)
    {
        _tigerApiFixture = tigerApiFixture;
    }

    [Fact]
    public async Task Dado_Request_Valido_Retorna_Created()
    {
        //Validar se há essa solicitação na base
        //Enviar requisição com a soliticação
        //Validar se a solicitação foi salva

        // Act
        var updateFinancialDataAsString = System.Text.Json.JsonSerializer.Serialize(_command);
        using var stringContent = new StringContent(updateFinancialDataAsString, Encoding.UTF8, "application/json");
        await _tigerApiFixture.Client.PostAsync("/creditos", stringContent);

        var products = MongoDbFixture.Produtos();
        Assert.Equal(products.Count, 1);

        var consumed = RabbitMqFixture.Produtos();
        Assert.Equal(consumed.Count, 1);
    }
}