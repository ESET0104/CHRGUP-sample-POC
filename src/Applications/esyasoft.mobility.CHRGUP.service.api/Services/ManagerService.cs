using esyasoft.mobility.CHRGUP.service.api.DTOs.Users;
using esyasoft.mobility.CHRGUP.service.api.Infrastructure.Security;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.core.Models;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;

namespace esyasoft.mobility.CHRGUP.service.api.Services
{
    public class ManagerService : IManagerService
    {
        private readonly AppDbContext _db;

        public ManagerService(AppDbContext db)

        {

            _db = db;

        }

        public async Task<UserResponseDto> CreateAsync(CreateUserDto dto)

        {

            if (await _db.managers.AnyAsync(m => m.Email == dto.Email))

                throw new InvalidOperationException("Manager with this email already exists");

            var manager = new Manager

            {

                Id = Nanoid.Generate(size: 10),

                Username = dto.Username,

                Email = dto.Email,

                Password = PasswordHasher.Hash(dto.Password),

                Status = dto.Status,

                Company = dto.Company,

                Department = dto.Department,

                CreatedAt = DateTime.Now

            };

            _db.managers.Add(manager);

            await _db.SaveChangesAsync();

            return Map(manager);

        }

        public async Task<List<UserResponseDto>> GetAllAsync()

        {

            return await _db.managers

                .OrderByDescending(m => m.CreatedAt)

                .Select(m => Map(m))

                .ToListAsync();

        }

        public async Task<UserResponseDto> GetByIdAsync(string id)

        {

            var manager = await _db.managers.FindAsync(id)

                ?? throw new KeyNotFoundException("Manager not found");

            return Map(manager);

        }

        public async Task<UserResponseDto> UpdateAsync(string id, UpdateUserDto dto)

        {

            var manager = await _db.managers.FindAsync(id)

                ?? throw new KeyNotFoundException("Manager not found");

            manager.Username = dto.Username ?? manager.Username;

            manager.Email = dto.Email ?? manager.Email;

            manager.Status = dto.Status ?? manager.Status;

            manager.Company = dto.Company ?? manager.Company;

            manager.Department = dto.Department ?? manager.Department;

            manager.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return Map(manager);

        }

        public async Task DeleteAsync(string id)

        {

            var manager = await _db.managers.FindAsync(id)

                ?? throw new KeyNotFoundException("Manager not found");

            _db.managers.Remove(manager);

            await _db.SaveChangesAsync();

        }

        private static UserResponseDto Map(Manager m) => new()

        {

            Id = m.Id,

            Username = m.Username,

            Email = m.Email,

            Status = m.Status,

            Company = m.Company,

            Department = m.Department,

            CreatedAt = m.CreatedAt

        };

    }
}
