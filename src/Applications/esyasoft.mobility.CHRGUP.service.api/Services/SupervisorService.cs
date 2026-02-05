using esyasoft.mobility.CHRGUP.service.api.DTOs.Users;
using esyasoft.mobility.CHRGUP.service.api.Infrastructure.Security;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.core.Models;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;

namespace esyasoft.mobility.CHRGUP.service.api.Services
{
    public class SupervisorService : ISupervisorService
    {
        private readonly AppDbContext _db;

        public SupervisorService(AppDbContext db)

        {

            _db = db;

        }

        public async Task<UserResponseDto> CreateAsync(CreateUserDto dto)

        {

            if (await _db.supervisors.AnyAsync(s => s.Email == dto.Email))

                throw new InvalidOperationException("Supervisor with this email already exists");

            var supervisor = new Supervisor

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

            _db.supervisors.Add(supervisor);

            await _db.SaveChangesAsync();

            return Map(supervisor);

        }

        public async Task<List<UserResponseDto>> GetAllAsync()

        {

            return await _db.supervisors

                .OrderByDescending(s => s.CreatedAt)

                .Select(s => Map(s))

                .ToListAsync();

        }

        public async Task<UserResponseDto> GetByIdAsync(string id)

        {

            var supervisor = await _db.supervisors.FindAsync(id)

                ?? throw new KeyNotFoundException("Supervisor not found");

            return Map(supervisor);

        }

        public async Task<UserResponseDto> UpdateAsync(string id, UpdateUserDto dto)

        {

            var supervisor = await _db.supervisors.FindAsync(id)

                ?? throw new KeyNotFoundException("Supervisor not found");

            supervisor.Username = dto.Username ?? supervisor.Username;

            supervisor.Email = dto.Email ?? supervisor.Email;

            supervisor.Status = dto.Status ?? supervisor.Status;

            supervisor.Company = dto.Company ?? supervisor.Company;

            supervisor.Department = dto.Department ?? supervisor.Department;

            supervisor.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return Map(supervisor);

        }

        public async Task DeleteAsync(string id)

        {

            var supervisor = await _db.supervisors.FindAsync(id)

                ?? throw new KeyNotFoundException("Supervisor not found");

            _db.supervisors.Remove(supervisor);

            await _db.SaveChangesAsync();

        }

        private static UserResponseDto Map(Supervisor s) => new()

        {

            Id = s.Id,

            Username = s.Username,

            Email = s.Email,

            Status = s.Status,

            Company = s.Company,

            Department = s.Department,

            CreatedAt = s.CreatedAt

        };

    }
}
