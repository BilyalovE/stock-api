using Grpc.Core;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using OzonEdu.StockApi.Controllers;
using OzonEdu.StockApi.Grpc;
using OzonEdu.StockApi.HttpModels;
using OzonEdu.StockApi.Services;
using OzonEdu.StockApi.Services.Interfaces;

namespace OzonEdu.StockApi.GrpcServices;

public class StockApiGrpcService : StockApiGrpc.StockApiGrpcBase
{
    private readonly IStockService _stockService;

    public StockApiGrpcService(IStockService stockService)
    { 
        _stockService = stockService;   
    }

    public override async Task<GetAllApiResponce> GetAllStockItems(GetAllApiRequest request, 
        ServerCallContext context)
    {
        var stockItems = await _stockService.GetAll(context.CancellationToken);
        return new GetAllApiResponce
        {
            Stocks =
            {
                stockItems.Select(x => new GetAllStockApiResponseUnit() {
                    ItemId = x.ItemId,
                    Quantity = x.Quantity,
                    ItemName = x.ItemName,
                })
            }
        };
    }
    
    public async Task<GetAllWithNullsApiResponce> GetAllStockItemsWitnNulls(Empty request, 
        ServerCallContext context)
    {
        var stockItems = await _stockService.GetAll(context.CancellationToken);
        return new GetAllWithNullsApiResponce 
        {
            Stocks =
            {
                stockItems.Select(x => new GetAllStockWithNullApiResponseUnit() {
                    ItemId = x.ItemId,
                    Quantity = x.Quantity,
                    ItemName = x.ItemName,
                })
            }
        };
    }
    
    public async Task<GetAllStockItemsMapResponce> GetAllMapStockItems(Empty request, 
        ServerCallContext context)
    {
        var stockItems = await _stockService.GetAll(context.CancellationToken);
        return new GetAllStockItemsMapResponce
        {
            Stocks =
            {
                stockItems.ToDictionary(x => x.ItemId, x => new GetAllStockApiResponseUnit() {
                    ItemId = x.ItemId,
                    Quantity = x.Quantity,
                    ItemName = x.ItemName,
                })
            }
        };
    }

    public override Task<Empty> AddStockItem(AddStockItemRequest request, ServerCallContext context)
    {
        throw new RpcException(
            new Status(StatusCode.InvalidArgument, "validation failed"),
            // валидация не прошла по тому...что  value может быть json 
            new Metadata {new Metadata.Entry("key", "value1111")});
    }

    public override async Task GetAllStockItemsStreaming
    (GetAllApiRequest request, 
        IServerStreamWriter<GetAllStockApiResponseUnit> responseStream, 
        ServerCallContext context)
    {
        var stockItems = await _stockService.GetAll(context.CancellationToken);
        foreach (var item in stockItems)
        {

            if (context.CancellationToken.IsCancellationRequested)
                break;
             
            await responseStream.WriteAsync(new GetAllStockApiResponseUnit
            {
                ItemId = item.ItemId,
                Quantity = item.Quantity,
                ItemName = item.ItemName,
            });
        } 
    }

    public override async Task<Empty> AddStockItemsStreaming(IAsyncStreamReader<AddStockItemRequest> requestStream,
        ServerCallContext context)
    {
        while (!context.CancellationToken.IsCancellationRequested)
        {
            await requestStream.MoveNext();
            var currentItem = requestStream.Current;
            await _stockService.Add(new StockItemCreationModel
                {
                    Quantity = currentItem.Quantity,
                    ItemName = currentItem.ItemName,
                },
                context.CancellationToken);
        }
        return new Empty();
    }
} 