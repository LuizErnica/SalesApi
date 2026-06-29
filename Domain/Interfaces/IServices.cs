using SalesApi.DTOs;

namespace SalesApi.Domain.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserResponseDto>> GetAllAsync();
    Task<UserResponseDto?> GetByIdAsync(int id);
    Task<UserResponseDto> CreateAsync(CreateUserDto dto);
    Task<UserResponseDto> UpdateAsync(int id, UpdateUserDto dto);
    Task DeleteAsync(int id);
}

public interface IProductService
{
    Task<IEnumerable<ProductResponseDto>> GetAllAsync();
    Task<ProductResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<ProductResponseDto>> GetByCategoryAsync(string category);
    Task<ProductResponseDto> CreateAsync(CreateProductDto dto);
    Task<ProductResponseDto> UpdateAsync(int id, UpdateProductDto dto);
    Task DeleteAsync(int id);
}

public interface ISaleService
{
    Task<IEnumerable<SaleResponseDto>> GetAllAsync();
    Task<SaleResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<SaleResponseDto>> GetByUserIdAsync(int userId);
    Task<SaleResponseDto> CreateAsync(CreateSaleDto dto);
    Task<SaleResponseDto> UpdateStatusAsync(int id, string status);
    Task DeleteAsync(int id);
}

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
}
