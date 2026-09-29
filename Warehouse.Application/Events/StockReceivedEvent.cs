using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Application.Events;

public record StockReceivedEvent(Guid ItemId, Guid LocationId, int Quantity, DateTime OccuresAtUtc);

