using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Domain.Entities;


public class Item
{
    public Guid Id { get; private set; }
    public string Sku { get; private set; }
    public string Name { get; private set; }

    private Item() { }
    public Item(Guid id, string sku, string name)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id не может быть пустым", nameof(id));
        if (string.IsNullOrWhiteSpace(sku)) 
            throw new ArgumentException("Sku не может быть пустым", nameof(sku));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name не может быть пустым", nameof(name));
        Id = id;
        Sku = sku;
        Name = name;
    }
}
