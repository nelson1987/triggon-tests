using Triggon.Core;
using Triggon.Core.Entities;
using Triggon.Core.Repositories;

namespace Triggon.Api;
public static class Dependencies
{
    public static IServiceCollection AddRepository(this IServiceCollection services)
    {
        services.AddScoped<IMongoContext, MongoContext>();
        services.AddScoped<IMongoRepository<Solicitacao>, SolicitacaoRepository>();
        return services;
    }
}