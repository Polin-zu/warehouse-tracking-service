namespace Warehouse.Api.Contracts;


public record ReceiveStockRequest(Guid ItemId, Guid LocationId, int Quantity);
public record WithdrawStockRequest(Guid ItemId, Guid LocationId, int Quantity);
public record MoveStockRequest(Guid ItemId, Guid FromLocationId, Guid ToLocationId, int Quantity);
