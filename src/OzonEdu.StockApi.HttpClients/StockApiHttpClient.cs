using System.Text.Json;
using OzonEdu.StockApi.HttpModels;

namespace OzonEdu.StockApi.HttpClients;

public interface IStockApiHttpClient
{
    Task<List<StockItemResponce>> GetAll(CancellationToken token);
}
public class StockApiHttpClient : IStockApiHttpClient
{
    private readonly HttpClient _httpClient; 

    public StockApiHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<StockItemResponce>> GetAll(CancellationToken token)
    {
        using var response = await _httpClient.GetAsync("v1/api/stock", token);
        var body = await response.Content.ReadAsStringAsync(token);
        return JsonSerializer.Deserialize<List<StockItemResponce>>(body);
    }
}