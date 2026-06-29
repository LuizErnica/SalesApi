using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesApi.Domain.Interfaces;
using SalesApi.DTOs;

namespace SalesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalesController : ControllerBase
{
    private readonly ISaleService _saleService;

    public SalesController(ISaleService saleService) => _saleService = saleService;

    /// <summary>List all sales (Admin)</summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<SaleResponseDto>>> GetAll()
    {
        var sales = await _saleService.GetAllAsync();
        return Ok(sales);
    }

    /// <summary>Get sale by ID</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SaleResponseDto>> GetById(int id)
    {
        var sale = await _saleService.GetByIdAsync(id);
        if (sale is null) return NotFound(new { message = $"Sale {id} not found." });

        var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (!User.IsInRole("Admin") && sale.UserId != currentUserId)
            return Forbid();

        return Ok(sale);
    }

    /// <summary>List sales from an user</summary>
    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<IEnumerable<SaleResponseDto>>> GetByUser(int userId)
    {
        var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (!User.IsInRole("Admin") && currentUserId != userId)
            return Forbid();

        var sales = await _saleService.GetByUserIdAsync(userId);
        return Ok(sales);
    }

    /// <summary>Create new sale</summary>
    [HttpPost]
    public async Task<ActionResult<SaleResponseDto>> Create([FromBody] CreateSaleDto dto)
    {
        // Customer can only create sale for himself
        var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (!User.IsInRole("Admin") && dto.UserId != currentUserId)
            return Forbid();

        try
        {
            var created = await _saleService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Update sale status (Admin)</summary>
    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<SaleResponseDto>> UpdateStatus(int id, [FromBody] UpdateSaleStatusDto dto)
    {
        try
        {
            var updated = await _saleService.UpdateStatusAsync(id, dto.Status);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Delete sale (Admin)</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _saleService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
