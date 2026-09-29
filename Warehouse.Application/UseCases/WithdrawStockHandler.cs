using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Warehouse.Application.Abstractions;
using Warehouse.Application.Events;

namespace Warehouse.Application.UseCases;

public class WithdrawStockHandler
{
    private readonly IStockRepository _stockRepository;
    private readonly IEventPublisher _eventPublisher;
    public WithdrawStockHandler(IStockRepository stockRepository, IEventPublisher eventPublisher)
    {
        _stockRepository = stockRepository;
        _eventPublisher = eventPublisher;
    }
    public async Task HandleAsync(Guid itemId, Guid locationId, int quantity, CancellationToken cancellationToken)
    {
        var stockRecord = await _stockRepository.GetByItemAndLocationAsync(itemId, locationId, cancellationToken);
        if (stockRecord is null)
        {
            throw new InvalidOperationException($"Остаток для товара {{itemId}} в ячейке {{locationId}} не найден");

        }
        stockRecord.Withdraw(quantity);
        await _stockRepository.SaveChangesAsync(cancellationToken);

        var stockEvent = new StockWithdrawnEvent(itemId, locationId, quantity, DateTime.UtcNow);
        await _eventPublisher.PublishAsync("stock-events", stockEvent, cancellationToken);

    }
}
