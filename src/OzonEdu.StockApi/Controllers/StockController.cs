using Microsoft.AspNetCore.Mvc;
using OzonEdu.StockApi.Services;
using OzonEdu.StockApi.HttpModels;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace OzonEdu.StockApi.Controllers;

[ApiController]
[Route("v1/api/stocks")]
public class StockController : ControllerBase
{
    private IStockService _stockService;

    public StockController(IStockService stockService)
    {
        _stockService = stockService;
    }

    /// <summary>
    /// Возвращает все сущности
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<List<StockItem>>> GetAll(CancellationToken token)
    {
        var stockItems = await _stockService.GetAll(token);
        return Ok(stockItems);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<StockItem>> GetById(long id, CancellationToken token)
    {
        var stockItem = await _stockService.GetById(id, token);
        if (stockItem is null)
        {
            return NotFound();
        }

        return Ok(stockItem);
    }

    [HttpPost]
    public async Task<ActionResult<StockItem>> Add(StockItemModel model, CancellationToken token)
    {
        var createdStockItem = await _stockService.Add(new StockItemCreationModel
        {
            ItemName = model.ItemName,
            Quantity = model.Quantity,
        }, token);
        return Ok(createdStockItem);
    }
}