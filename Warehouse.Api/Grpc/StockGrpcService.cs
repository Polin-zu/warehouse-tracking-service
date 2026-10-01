using Grpc.Core;
using StackExchange.Redis;

namespace Warehouse.Api.Grpc;

public class StockGrpcService : StockReceive.StockReceiveBase
{
    private readonly IConnectionMultiplexer _redis;
    public StockGrpcService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }
    public override async Task<GetStockLevelResponse> GetStockLevel(
        GetStockLevelRequest request,
        ServerCallContext context)
    {
        if (!Guid.TryParse(request.ItemId, out var itemId))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "item_id должен быть валидным Guid"));
        if (!Guid.TryParse(request.LocationId, out var locationId))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "location_id должен быть валидным Guid"));
        var db = _redis.GetDatabase();
        var stockKey = $"stock:{itemId}:{locationId}";
        var value = await db.StringGetAsync(stockKey);

        var quantity = value.HasValue ? (int)value : 0;
        return new GetStockLevelResponse { Quantity = quantity };
    }
}
