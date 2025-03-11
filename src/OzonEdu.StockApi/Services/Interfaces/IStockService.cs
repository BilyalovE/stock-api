using OzonEdu.StockApi.HttpModels;

namespace OzonEdu.StockApi.Services.Interfaces;

public interface IStockService
{
    Task<List<StockItem>> GetAll(CancellationToken token);
    public Task<StockItem> GetById(long itemId, CancellationToken _);
    public Task<StockItem> Add(StockItemCreationModel stockItem, CancellationToken token);
}