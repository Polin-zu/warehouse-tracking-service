using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Warehouse.Domain.Entities;

namespace Warehouse.Infrastructure.Persistence.Configurations;

public class StockRecordConfiguration : IEntityTypeConfiguration<StockRecord>
{
    public void Configure(EntityTypeBuilder<StockRecord> builder)
    {
        builder.ToTable("stock_records");

        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.ItemId).IsRequired(); //not null
        builder.Property(s => s.LocationId).IsRequired();
        builder.Property(s => s.Quantity).IsRequired();
        
        //не может быть двух записей StockRecord с одинаковыми ItemId и LocationId
        builder.HasIndex(s => new { s.ItemId, s.LocationId }).IsUnique(); 
    }
}
