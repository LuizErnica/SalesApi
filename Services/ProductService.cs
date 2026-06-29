using SalesApi.Domain.Entities;
using SalesApi.Domain.Interfaces;
using SalesApi.DTOs;

namespace SalesApi.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo) => _repo = repo;

    public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
    {
        var products = await _repo.GetAllAsync();
        return products.Select(ToDto);
    }

    public async Task<ProductResponseDto?> GetByIdAsync(int id)
    {
        var product = await _repo.GetByIdAsync(id);
        return product is null ? null : ToDto(product);
    }

    public async Task<IEnumerable<ProductResponseDto>> GetByCategoryAsync(string category)
    {
        var products = await _repo.GetByCategoryAsync(category);
        return products.Select(ToDto);
    }

    public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            Category = dto.Category
        };

        var created = await _repo.CreateAsync(product);
        return ToDto(created);
    }

    public async Task<ProductResponseDto> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Product {id} not found.");

        if (dto.Name is not null) product.Name = dto.Name;
        if (dto.Description is not null) product.Description = dto.Description;
        if (dto.Price is not null) product.Price = dto.Price.Value;
        if (dto.StockQuantity is not null) product.StockQuantity = dto.StockQuantity.Value;
        if (dto.Category is not null) product.Category = dto.Category;
        if (dto.IsActive is not null) product.IsActive = dto.IsActive.Value;

        var updated = await _repo.UpdateAsync(product);
        return ToDto(updated);
    }

    public async Task DeleteAsync(int id)
    {
        if (!await _repo.ExistsAsync(id))
            throw new KeyNotFoundException($"Product {id} not found.");
        await _repo.DeleteAsync(id);
    }

    private static ProductResponseDto ToDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Price = p.Price,
        StockQuantity = p.StockQuantity,
        Category = p.Category,
        IsActive = p.IsActive,
        CreatedAt = p.CreatedAt
    };
}
