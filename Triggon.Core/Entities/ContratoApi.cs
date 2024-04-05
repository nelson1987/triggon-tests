using Triggon.Core.Services;

namespace Triggon.Core.Entities;

public class ContratoApi : IContratoApi
{
    public async Task<bool> Autorizar(Conta conta, CancellationToken cancellationToken = default)
    {
        string uri = "";
        var client = new HttpClient();
        var response = await client.GetAsync(uri);
        if (!response.IsSuccessStatusCode)
        {
            throw new TriggonException("");
        }
        //var dados = await response.Content.ReadAsStringAsync<bool>();
        return true;
    }
}