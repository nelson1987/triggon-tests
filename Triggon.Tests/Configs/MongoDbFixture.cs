using MongoDB.Driver;
using Triggon.Core;
using Triggon.Core.Repositories;

namespace Triggon.Tests.Configs;
public static class MongoDbFixture
{
    // public static AssetsManagerDbContext Client { get; private set; }
    public static async Task ClearDatabase(string database,
        string collection)
    {
        await Client
            .GetDatabase(database)
            .DropCollectionAsync(collection);
    }
    public static async Task CreateDatabase(string database,
        string collection)
    {
        await Client
            .GetDatabase(database)
            .CreateCollectionAsync(collection);
    }
    public static MongoClient Client
    {
        get
        {
            var connectionString = "mongodb://root:password@localhost:27017/";
            return new MongoClient(connectionString);
        }
    }

    public static IMongoCollection<T> Collection<T>(string database,
        string collection) where T : class
    {
        return Client
            .GetDatabase(database)
            .GetCollection<T>(collection);
    }
    public static IMongoContext Context()
    {
        return new MongoContext();
    }
}