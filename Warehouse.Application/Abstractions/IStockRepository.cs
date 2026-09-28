using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Warehouse.Domain.Entities;

namespace Warehouse.Application.Abstractions;


public interface IStockRepository
{
    Task<StockRecord?> GetByItemAndLocationAsync(Guid itemId, Guid locationId, CancellationToken cancellationToken);
    Task AddAsync(StockRecord stockRecord, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
