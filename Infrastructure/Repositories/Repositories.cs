using Microsoft.EntityFrameworkCore;
using SalesApi.Domain.Entities;
using SalesApi.Domain.Interfaces;
using SalesApi.Infrastructure.Data;

namespace SalesApi.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _ctx;
    public UserRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<User>> GetAllAsync() =>
        await _ctx.Users.AsNoTracking().ToListAsync();

    public async Task<User?> GetByIdAsync(int id) =>
        await _ctx.Users.FindAsync(id);

    public async Task<User?> GetByEmailAsync(string email) =>
        await _ctx.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User> CreateAsync(User user)
    {
        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        _ctx.Users.Update(user);
        await _ctx.SaveChangesAsync();
        return user;
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _ctx.Users.FindAsync(id)
            ?? throw new KeyNotFoundException($"User {id} not found.");
        _ctx.Users.Remove(user);
        await _ctx.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id) =>
        await _ctx.Users.AnyAsync(u => u.Id == id);
}

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _ctx;
    public ProductRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<Product>> GetAllAsync() =>
        await _ctx.Products.AsNoTracking().ToListAsync();

    public async Task<Product?> GetByIdAsync(int id) =>
        await _ctx.Products.FindAsync(id);

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category) =>
        await _ctx.Products
            .Where(p => p.Category.ToLower() == category.ToLower())
            .AsNoTracking()
            .ToListAsync();

    public async Task<Product> CreateAsync(Product product)
    {
        _ctx.Products.Add(product);
        await _ctx.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        _ctx.Products.Update(product);
        await _ctx.SaveChangesAsync();
        return product;
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _ctx.Products.FindAsync(id)
            ?? throw new KeyNotFoundException($"Product {id} not found.");
        _ctx.Products.Remove(product);
        await _ctx.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id) =>
        await _ctx.Products.AnyAsync(p => p.Id == id);
}

public class SaleRepository : ISaleRepository
{
    private readonly AppDbContext _ctx;
    public SaleRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<Sale>> GetAllAsync() =>
        await _ctx.Sales
            .Include(s => s.User)
            .Include(s => s.Items).ThenInclude(i => i.Product)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Sale?> GetByIdAsync(int id) =>
        await _ctx.Sales
            .Include(s => s.User)
            .Include(s => s.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<IEnumerable<Sale>> GetByUserIdAsync(int userId) =>
        await _ctx.Sales
            .Include(s => s.User)
            .Include(s => s.Items).ThenInclude(i => i.Product)
            .Where(s => s.UserId == userId)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Sale> CreateAsync(Sale sale)
    {
        _ctx.Sales.Add(sale);
        await _ctx.SaveChangesAsync();
        return sale;
    }

    public async Task<Sale> UpdateAsync(Sale sale)
    {
        _ctx.Sales.Update(sale);
        await _ctx.SaveChangesAsync();
        return sale;
    }

    public async Task DeleteAsync(int id)
    {
        var sale = await _ctx.Sales.FindAsync(id)
            ?? throw new KeyNotFoundException($"Sale {id} not found.");
        _ctx.Sales.Remove(sale);
        await _ctx.SaveChangesAsync();
    }
}
