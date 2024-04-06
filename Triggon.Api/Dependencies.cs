using Triggon.Core;
using Triggon.Core.Contexts;
using Triggon.Core.Entities;
using Triggon.Core.Repositories;

namespace Triggon.Api;
public static class Dependencies
{
    public static IServiceCollection AddRepository(this IServiceCollection services)
    {
        services.AddSingleton(_ => new MyApplicationContextOptions()
        {
            ConnectionString = "mongodb://root:password@localhost:27017/",
            DatabaseName = "warehouseTests"
        });
        services.AddScoped<IMyApplicationContext, MyApplicationContext>();
        services.AddScoped<IMongoRepository<Usuario>, UserRepository>();
        services.AddScoped<IMongoRepository<Solicitacao>, SolicitacaoRepository>();
        return services;
    }
}