using esyasoft.mobility.CHRGUP.service.api.DTOs.Users;
using esyasoft.mobility.CHRGUP.service.api.Infrastructure.Security;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.core.Models;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;

namespace esyasoft.mobility.CHRGUP.service.api.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _db;

        public AdminService(AppDbContext db)

        {

            _db = db;

        }

        public async Task<UserResponseDto> CreateAsync(CreateUserDto dto)

        {

            if (await _db.admins.AnyAsync(a => a.Email == dto.Email))

                throw new InvalidOperationException("Admin with this email already exists");

            var admin = new Admin

            {

                Id = Nanoid.Generate(size: 10),

                Username = dto.Username,

                Email = dto.Email,

                //Password = dto.Password,

                Password = PasswordHasher.Hash(dto.Password),

                Status = dto.Status,

                Company = dto.Company,

                Department = dto.Department,

                CreatedAt = DateTime.Now

            };

            _db.admins.Add(admin);

            await _db.SaveChangesAsync();

            return Map(admin);

        }

        public async Task<List<UserResponseDto>> GetAllAsync()

        {

            return await _db.admins

                .OrderByDescending(a => a.CreatedAt)

                .Select(a => Map(a))

                .ToListAsync();

        }

        public async Task<UserResponseDto> GetByIdAsync(string id)

        {

            var admin = await _db.admins.FindAsync(id)

                ?? throw new KeyNotFoundException("Admin not found");

            return Map(admin);

        }

        public async Task<UserResponseDto> UpdateAsync(string id, UpdateUserDto dto)

        {

            var admin = await _db.admins.FindAsync(id)

                ?? throw new KeyNotFoundException("Admin not found");

            if (dto.Username != null) admin.Username = dto.Username;

            if (dto.Email != null) admin.Email = dto.Email;

            if (dto.Status != null) admin.Status = dto.Status;

            if (dto.Company != null) admin.Company = dto.Company;

            if (dto.Department != null) admin.Department = dto.Department;

            admin.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return Map(admin);

        }

        public async Task DeleteAsync(string id)

        {

            var admin = await _db.admins.FindAsync(id)

                ?? throw new KeyNotFoundException("Admin not found");

            _db.admins.Remove(admin);

            await _db.SaveChangesAsync();

        }

        private static UserResponseDto Map(Admin a) => new()

        {

            Id = a.Id,

            Username = a.Username,

            Email = a.Email,

            Status = a.Status,

            Company = a.Company,

            Department = a.Department,

            CreatedAt = a.CreatedAt

        };

    }
}
