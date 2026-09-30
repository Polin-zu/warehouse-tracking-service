using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Application.Events;

public record StockMovedEvent(Guid EventId, Guid ItemId, Guid FromLocationId, 
    Guid ToLocationId, int Quantity, DateTime OccuresAtUtc);


