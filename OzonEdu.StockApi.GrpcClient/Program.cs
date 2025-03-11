using System;
using System.Threading;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using OzonEdu.StockApi.Grpc;

// нужен канал по которому клиент будет образаться к API
using var channel = GrpcChannel.ForAddress("http://localhost:5001");
var client = new StockApiGrpc.StockApiGrpcClient(channel); 

// var response = await client.GetAllStockItemsAsync(new GetAllApiRequest(), cancellationToken: CancellationToken.None);
// foreach (var item in response.Stocks)
// {
//     Console.WriteLine($"item-id {item.ItemId}  quantity {item.Quantity}");
// }

// try
// { 
//     client.AddStockItem(new AddStockItemRequest {Quantity = 100, ItemName = "Test"});
// }
// catch (RpcException ex)
// {
//     var metadata = ex.Trailers;
//     metadata.FirstOrDefault(x => x.Key == "key");
//     Console.WriteLine(ex);
// }

var streamingCall = client.GetAllStockItemsStreaming(new GetAllApiRequest());
await foreach (var stockItem in streamingCall.ResponseStream.ReadAllAsync())
{
    Console.WriteLine($"item-id {stockItem.ItemId}  quantity {stockItem.Quantity}");
}

 
// while (await streamingCall.ResponseStream.MoveNext())
// {
//     var stockItem = streamingCall.ResponseStream.Current;
//     Console.WriteLine($"item-id {stockItem.ItemId}  quantity {stockItem.Quantity}");
// }

var clientStreamCall = client.AddStockItemsStreaming(cancellationToken: CancellationToken.None);
await clientStreamCall.RequestStream.WriteAsync(new AddStockItemRequest
{
    Quantity = 100,
    ItemName = "Shoes"
});
await clientStreamCall.RequestStream.WriteAsync(new AddStockItemRequest
{
    Quantity = 39,
    ItemName = "Cap"
});

await clientStreamCall.RequestStream.CompleteAsync();
Console.ReadKey();

