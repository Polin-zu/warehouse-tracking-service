using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Warehouse.Domain.Entities;
using Warehouse.Domain.Exceptions;
using Xunit;


namespace Warehouse.Domain.Tests;


public class StockRecordTests
{
    [Fact]
    public void Receive_ShouldIncreaseQuantity()
    {
        var stock = new StockRecord(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), initialQuantity: 10);
        stock.Receive(5);
        Assert.Equal(15, stock.Quantity);
    }

    [Fact]
    public void Withdraw_ShouldDecreaseQuantity_WhenEnoughStock()
    {
        var stock = new StockRecord(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), initialQuantity: 67);
        stock.Withdraw(6);
        Assert.Equal(61, stock.Quantity);
    }
    [Fact]
    public void Withdraw_ShouldThrow_WhenNotEnoughStock()
    {
        var stock = new StockRecord(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), initialQuantity: 3);
        Assert.Throws<InsufficientStockException>(() => stock.Withdraw(5));

    }
    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Receive_ShouldThrow_WhenQuantityNotPositive(int invalidQuantity)
    {
        var stock = new StockRecord(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), initialQuantity: 10);
        Assert.Throws<ArgumentOutOfRangeException>(() => stock.Receive(invalidQuantity));

    }
}


