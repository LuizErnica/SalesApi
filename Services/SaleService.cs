using SalesApi.Domain.Entities;
using SalesApi.Domain.Interfaces;
using SalesApi.DTOs;

namespace SalesApi.Services;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepo;
    private readonly IProductRepository _productRepo;
    private readonly IUserRepository _userRepo;

    public SaleService(ISaleRepository saleRepo, IProductRepository productRepo, IUserRepository userRepo)
    {
        _saleRepo = saleRepo;
        _productRepo = productRepo;
        _userRepo = userRepo;
    }

    public async Task<IEnumerable<SaleResponseDto>> GetAllAsync()
    {
        var sales = await _saleRepo.GetAllAsync();
        return sales.Select(ToDto);
    }

    public async Task<SaleResponseDto?> GetByIdAsync(int id)
    {
        var sale = await _saleRepo.GetByIdAsync(id);
        return sale is null ? null : ToDto(sale);
    }

    public async Task<IEnumerable<SaleResponseDto>> GetByUserIdAsync(int userId)
    {
        var sales = await _saleRepo.GetByUserIdAsync(userId);
        return sales.Select(ToDto);
    }

    public async Task<SaleResponseDto> CreateAsync(CreateSaleDto dto)
    {
        if (!await _userRepo.ExistsAsync(dto.UserId))
            throw new KeyNotFoundException($"User {dto.UserId} not found.");

        var sale = new Sale { UserId = dto.UserId, Notes = dto.Notes };

        foreach (var itemDto in dto.Items)
        {
            var product = await _productRepo.GetByIdAsync(itemDto.ProductId)
                ?? throw new KeyNotFoundException($"Product {itemDto.ProductId} not found.");

            if (product.StockQuantity < itemDto.Quantity)
                throw new InvalidOperationException(
                    $"Insufficient stock for '{product.Name}'. Available: {product.StockQuantity}.");

            sale.Items.Add(new SaleItem
            {
                ProductId = product.Id,
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price
            });

            product.StockQuantity -= itemDto.Quantity;
            await _productRepo.UpdateAsync(product);
        }

        sale.TotalAmount = sale.Items.Sum(i => i.Quantity * i.UnitPrice);

        var created = await _saleRepo.CreateAsync(sale);

        // Reload with navigation properties
        var full = await _saleRepo.GetByIdAsync(created.Id);
        return ToDto(full!);
    }

    public async Task<SaleResponseDto> UpdateStatusAsync(int id, string status)
    {
        var validStatuses = new[] { "Pending", "Confirmed", "Cancelled" };
        if (!validStatuses.Contains(status))
            throw new ArgumentException($"Invalid status. Use: {string.Join(", ", validStatuses)}");

        var sale = await _saleRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Sale {id} not found.");

        // Restore stock on cancellation
        if (status == "Cancelled" && sale.Status != "Cancelled")
        {
            foreach (var item in sale.Items)
            {
                var product = await _productRepo.GetByIdAsync(item.ProductId);
                if (product is not null)
                {
                    product.StockQuantity += item.Quantity;
                    await _productRepo.UpdateAsync(product);
                }
            }
        }

        sale.Status = status;
        await _saleRepo.UpdateAsync(sale);

        var updated = await _saleRepo.GetByIdAsync(id);
        return ToDto(updated!);
    }

    public async Task DeleteAsync(int id)
    {
        var sale = await _saleRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Sale {id} not found.");

        // Restore stock if not cancelled
        if (sale.Status != "Cancelled")
        {
            foreach (var item in sale.Items)
            {
                var product = await _productRepo.GetByIdAsync(item.ProductId);
                if (product is not null)
                {
                    product.StockQuantity += item.Quantity;
                    await _productRepo.UpdateAsync(product);
                }
            }
        }

        await _saleRepo.DeleteAsync(id);
    }

    private static SaleResponseDto ToDto(Sale s) => new()
    {
        Id = s.Id,
        UserId = s.UserId,
        UserName = s.User?.Name ?? string.Empty,
        SaleDate = s.SaleDate,
        TotalAmount = s.TotalAmount,
        Status = s.Status,
        Notes = s.Notes,
        Items = s.Items.Select(i => new SaleItemResponseDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ProductName = i.Product?.Name ?? string.Empty,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            Subtotal = i.Quantity * i.UnitPrice
        }).ToList()
    };
}
