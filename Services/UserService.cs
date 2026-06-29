using SalesApi.Domain.Entities;
using SalesApi.Domain.Interfaces;
using SalesApi.DTOs;

namespace SalesApi.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;

    public UserService(IUserRepository repo) => _repo = repo;

    public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
    {
        var users = await _repo.GetAllAsync();
        return users.Select(ToDto);
    }

    public async Task<UserResponseDto?> GetByIdAsync(int id)
    {
        var user = await _repo.GetByIdAsync(id);
        return user is null ? null : ToDto(user);
    }

    public async Task<UserResponseDto> CreateAsync(CreateUserDto dto)
    {
        var existing = await _repo.GetByEmailAsync(dto.Email);
        if (existing is not null)
            throw new InvalidOperationException("E-mail already registered.");

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role
        };

        var created = await _repo.CreateAsync(user);
        return ToDto(created);
    }

    public async Task<UserResponseDto> UpdateAsync(int id, UpdateUserDto dto)
    {
        var user = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"User {id} not found.");

        if (dto.Name is not null) user.Name = dto.Name;
        if (dto.Email is not null) user.Email = dto.Email;
        if (dto.Password is not null) user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        if (dto.IsActive is not null) user.IsActive = dto.IsActive.Value;

        var updated = await _repo.UpdateAsync(user);
        return ToDto(updated);
    }

    public async Task DeleteAsync(int id)
    {
        if (!await _repo.ExistsAsync(id))
            throw new KeyNotFoundException($"User {id} not found.");
        await _repo.DeleteAsync(id);
    }

    private static UserResponseDto ToDto(User u) => new()
    {
        Id = u.Id,
        Name = u.Name,
        Email = u.Email,
        Role = u.Role,
        IsActive = u.IsActive,
        CreatedAt = u.CreatedAt
    };
}
