using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Application.Events;

public record StockWithdrawnEvent(Guid EventId, Guid ItemId, Guid LocationId, int Quantity, DateTime OccuresAtUtc);

