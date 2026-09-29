using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Warehouse.Application.Abstractions;
using Warehouse.Domain.Entities;
using Warehouse.Infrastructure.Persistence;

namespace Warehouse.Infrastructure.Repositories;

public class StockRepository : IStockRepository
{
    private readonly WarehouseDbContext _context;
    public StockRepository(WarehouseDbContext context)
    {
        _context = context;
    }
    public async Task<StockRecord?> GetByItemAndLocationAsync(Guid itemId, Guid locationId, CancellationToken cancellationToken)
    {
        return await _context.StockRecords
            .FirstOrDefaultAsync(s => s.ItemId == itemId && s.LocationId == locationId, cancellationToken);
    }
    public async Task AddAsync(StockRecord stockRecord, CancellationToken cancellationToken)
    {
        await _context.StockRecords.AddAsync(stockRecord, cancellationToken);
    }
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
