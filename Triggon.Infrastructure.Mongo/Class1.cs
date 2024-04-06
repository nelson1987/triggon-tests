using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Triggon.Infrastructure.Mongo
{
    public static class MongoDependencies
    {
        public static void AddMongoDb(this IServiceCollection services)
        {
            var mongoClient = new MongoClient("<Your MongoDB Connection URI>");
            var dbContextOptions =
                new DbContextOptionsBuilder<MyDbContext>().UseMongoDB(mongoClient, "<Database Name");
            var db = new MyDbContext(dbContextOptions.Options);

        }
    }

    public abstract class MongoEntiyBase
    {
        public ObjectId Id { get; set; }
    }

    public class Customer : MongoEntiyBase
    {
        public String Name { get; set; }
        public String Order { get; set; }
    }

    public class MyDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; init; }
        public MyDbContext(DbContextOptions options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Customer>().ToCollection("customers");
        }
    }
}
