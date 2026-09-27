using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Warehouse.Domain.Entities;


public class StorageLocation
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public string Zone { get; private set; }
    private StorageLocation() { }
    public StorageLocation(Guid id, string code, string zone)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id не может быть пустым", nameof(id));
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code не может быть пустым", nameof(code));
        if (string.IsNullOrWhiteSpace(zone))
            throw new ArgumentNullException("Zone не может быть пустым", nameof(zone));
        Id = id;
        Code = code;
        Zone = zone;
    }

}
