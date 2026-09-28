using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Moq;
using Warehouse.Application.Abstractions;
using Warehouse.Application.UseCases;
using Warehouse.Domain.Entities;
using Xunit;

namespace Warehouse.Application.Tests;


public class ReceiveStockHandlerTests
{
    private readonly Mock<IStockRepository> _repoMock = new();
    private readonly ReceiveStockHandler _handler;

    public ReceiveStockHandlerTests()
    {
        _handler = new ReceiveStockHandler(_repoMock.Object);
    }
    [Fact]
    public async Task HandleAsync_ShouldCreateNewRecord_WhenNoneExists()
    {
        var itemId = Guid.NewGuid();
        var locationId = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByItemAndLocationAsync(itemId, locationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((StockRecord?)null);

        await _handler.HandleAsync(itemId, locationId, 5, CancellationToken.None);
        
        _repoMock.Verify(
            r => r.AddAsync(
                It.Is<StockRecord>(s => s.ItemId == itemId && s.LocationId == locationId && s.Quantity == 5),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    [Fact]
    public async Task HandleAsync_ShouldIncreaseQuantity_WhenRecordExists()
    {
        
        var itemId = Guid.NewGuid();
        var locationId = Guid.NewGuid();
        var existing = new StockRecord(Guid.NewGuid(), itemId, locationId, initialQuantity: 10);

        _repoMock
            .Setup(r => r.GetByItemAndLocationAsync(itemId, locationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        await _handler.HandleAsync(itemId, locationId, 5, CancellationToken.None);
        
        Assert.Equal(15, existing.Quantity);
        _repoMock.Verify(r => r.AddAsync(It.IsAny<StockRecord>(), It.IsAny<CancellationToken>()), Times.Never);
        _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
