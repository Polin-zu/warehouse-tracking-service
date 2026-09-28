using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Application.Abstractions;
namespace Warehouse.Application.UseCases;

public class MoveStockHandler
{
    private readonly IStockRepository _stockRepository;
    private readonly WithdrawStockHandler _withdrawHandler;
    private readonly ReceiveStockHandler _receiveHandler;

    public MoveStockHandler(IStockRepository stockRepository, WithdrawStockHandler withdrawHandler,
        ReceiveStockHandler receiveHandler)
    {
        _stockRepository = stockRepository;
        _withdrawHandler = withdrawHandler;
        _receiveHandler = receiveHandler;
    }
    public async Task HandleAsync(Guid itemId, Guid fromLocationId, Guid toLocationId,
        int quantity, CancellationToken cancellationToken)
    {
        await _withdrawHandler.HandleAsync(itemId, fromLocationId, quantity, cancellationToken); //временно, должно быть одной транзакцией
        await _receiveHandler.HandleAsync(itemId, toLocationId, quantity, cancellationToken);
    }
}
