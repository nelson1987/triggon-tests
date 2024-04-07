using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Triggon.Core.Contexts;
using Triggon.Infrastructure.Mongo;

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

    public static IMongoCollection<T> Collection<T>(string collection) where T : class
    {
        return Context()
            .GetGenericDatabase()
            .GetCollection<T>(collection);
    }
    public static IMyApplicationContext Context()
    {
        return new MyApplicationContext(new MyApplicationContextOptions()
        {
            ConnectionString = "mongodb://root:password@localhost:27017/",
            DatabaseName = "warehouseTests"
        });
    }
    public static ApplicationDbContext ApplicationDbContext()
    {
        DbContextOptionsBuilder opt = new DbContextOptionsBuilder();
        opt
                        .EnableSensitiveDataLogging()
                        .UseMongoDB("mongodb://root:password@localhost:27017/", "warehouseTests");

        return new ApplicationDbContext(opt.Options);
    }
    /*
     ApplicationDbContext>(opt =>
        {
            opt
                .EnableSensitiveDataLogging()
                .UseMongoDB("mongodb://root:password@localhost:27017/", "warehouseTests");
        });
     */
}