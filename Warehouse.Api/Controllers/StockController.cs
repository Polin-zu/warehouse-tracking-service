using Microsoft.AspNetCore.Mvc;
using Warehouse.Api.Contracts;
using Warehouse.Application.UseCases;
using Warehouse.Domain.Exceptions;


namespace Warehouse.Api.Controllers;

[ApiController]
[Route("api/stock")]
public class StockController : ControllerBase
{
    private readonly ReceiveStockHandler _receiveHandler;
    private readonly WithdrawStockHandler _withdrawHandler;
    private readonly MoveStockHandler _moveHandler;

    public StockController(ReceiveStockHandler receiveHandler, WithdrawStockHandler withdrawHandler, MoveStockHandler moveHandler)
    {
        _receiveHandler = receiveHandler;
        _withdrawHandler = withdrawHandler;
        _moveHandler = moveHandler;
    }

    [HttpPost("receive")]
    public async Task<IActionResult> Receive(ReceiveStockRequest request, CancellationToken cancellationToken)
    {
        await _receiveHandler.HandleAsync(request.ItemId, request.LocationId, request.Quantity, cancellationToken);
        return Ok();
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw(WithdrawStockRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _withdrawHandler.HandleAsync(request.ItemId, request.LocationId, request.Quantity, cancellationToken);
            return Ok();
        }
        catch (InsufficientStockException ex)
        {
            return BadRequest(ex.Message);
        }
        
    }

    [HttpPost("move")]
    public async Task<IActionResult> Move(MoveStockRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _moveHandler.HandleAsync(request.ItemId, request.FromLocationId, request.ToLocationId, request.Quantity, cancellationToken);
            return Ok();
        }
        catch (InsufficientStockException ex)
        {
            return BadRequest(ex.Message);
        }

    }

}
