using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Warehouse.Application.Abstractions;
using Warehouse.Domain.Entities; 

namespace Warehouse.Application.UseCases;


public class ReceiveStockHandler
{
    private readonly IStockRepository _stockRepository;
    public ReceiveStockHandler(IStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }
    public async Task HandleAsync(Guid itemId, Guid locationId, int quantity, CancellationToken cancellationToken)
    {
        var stockRecord = await _stockRepository.GetByItemAndLocationAsync(itemId, locationId, cancellationToken);
        if (stockRecord is null)
        {
            stockRecord = new StockRecord(Guid.NewGuid(), itemId, locationId, initialQuantity: quantity);
            await _stockRepository.AddAsync(stockRecord, cancellationToken);

        }
        else
        {
            stockRecord.Receive(quantity);
        }
        await _stockRepository.SaveChangesAsync(cancellationToken);
    }
}
