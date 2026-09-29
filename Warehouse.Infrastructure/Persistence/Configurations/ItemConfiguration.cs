using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Warehouse.Domain.Entities;

namespace Warehouse.Infrastructure.Persistence.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("items");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Sku).IsRequired().HasMaxLength(100); //not null
        builder.Property(s => s.Name).IsRequired().HasMaxLength(300);

        builder.HasIndex(s => s.Sku).IsUnique();
    }
}
