using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Warehouse.Domain.Exceptions;

namespace Warehouse.Domain.Entities;
public class StockRecord
{
    public Guid Id { get; private set; }
    public Guid ItemId { get; private set; }
    public Guid LocationId { get; private set; }
    public int Quantity { get; private set; }
    private StockRecord() { }
    public StockRecord(Guid id, Guid itemId, Guid locationId, int initialQuantity)

    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id не может быть пустым", nameof(id));
        if (initialQuantity < 0)
            throw new ArgumentOutOfRangeException(nameof(initialQuantity), "Количество не может быть отрицательным");
        
        Id = id;
        ItemId = itemId;
        LocationId = locationId;
        Quantity = initialQuantity;
    }

    public void Receive(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Количесвто приемки должно быть положительным");
        Quantity += quantity;
    }
    public void Withdraw(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Количесвто списания должно быть положительным");
        if (Quantity < quantity)
        {
            throw new InsufficientStockException(
                $"НЕдостаточно товара: доступно {Quantity}, запрошено {quantity}");

        }
        Quantity -= quantity;

    }
}
