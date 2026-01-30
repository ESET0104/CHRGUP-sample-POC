using esyasoft.mobility.CHRGUP.service.persistence.Data;
using esyasoft.mobility.CHRGUP.service.api.DTOs.Driver;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.core.Metadata;
using esyasoft.mobility.CHRGUP.service.core.Models;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;

namespace esyasoft.mobility.CHRGUP.service.api.Services
{
    public class DriverService : IDriverService
    {
        private readonly AppDbContext _db;

        public DriverService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<DriverResponseDto> CreateAsync(CreateDriverDto dto)
        {
            if (await _db.drivers.AnyAsync(d => d.Email == dto.Email))
                throw new InvalidOperationException("Driver with this email already exists");

            if (!string.IsNullOrEmpty(dto.VehicleId))
            {
                var vehicleExists = await _db.vehicles
                    .AnyAsync(v => v.Id == dto.VehicleId);

                if (!vehicleExists)
                    throw new InvalidOperationException("Vehicle not found");

                var alreadyAssigned = await _db.drivers
                    .AnyAsync(d => d.VehicleId == dto.VehicleId);

                if (alreadyAssigned)
                    throw new InvalidOperationException("Vehicle already assigned");
            }

            var driver = new Driver
            {
                Id = Nanoid.Generate(size: 10),
                FullName = dto.FullName,
                Email = dto.Email,
                Password = dto.Password, 
                RfidTag = Nanoid.Generate(size: 10),
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                Status = DriverStatus.Active,
                CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified),
                VehicleId = dto.VehicleId
            };

            _db.drivers.Add(driver);
            await _db.SaveChangesAsync();

            return Map(driver);
        }

        public async Task<List<DriverResponseDto>> GetAllAsync()
        {
            return await _db.drivers
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => Map(d))
                .ToListAsync();
        }

        public async Task<DriverResponseDto> GetByIdAsync(string id)
        {
            var driver = await _db.drivers.FirstOrDefaultAsync(d => d.Id == id)
                ?? throw new KeyNotFoundException("Driver not found");

            return Map(driver);
        }

        public async Task UpdateStatusAsync(string id, DriverStatus status)
        {
            var driver = await _db.drivers.FirstOrDefaultAsync(d => d.Id == id)
                ?? throw new KeyNotFoundException("Driver not found");

            driver.Status = status;
            driver.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

            await _db.SaveChangesAsync();
        }

        public async Task<Driver> AssignVehicleAsync(string driverId, string vehicleId)
        {
            var driver = await _db.drivers.FirstOrDefaultAsync(d => d.Id == driverId)
                ?? throw new KeyNotFoundException("Driver not found");

            var vehicleExists = await _db.vehicles.AnyAsync(v => v.Id == vehicleId);
            if (!vehicleExists)
                throw new InvalidOperationException("Vehicle not found");

            var alreadyAssigned = await _db.drivers
                .AnyAsync(d => d.VehicleId == vehicleId && d.Id != driverId);

            if (alreadyAssigned)
                throw new InvalidOperationException("Vehicle already assigned");

            driver.VehicleId = vehicleId;
            driver.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

            await _db.SaveChangesAsync();
            return driver;
        }

        private static DriverResponseDto Map(Driver d) => new()
        {
            Id = d.Id,
            FullName = d.FullName,
            Email = d.Email,
            Status = d.Status.ToString(),
            CreatedAt = d.CreatedAt
        };
    }
}
