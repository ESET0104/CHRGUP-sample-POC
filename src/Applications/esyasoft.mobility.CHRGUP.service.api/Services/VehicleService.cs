using esyasoft.mobility.CHRGUP.service.api.Data;
using esyasoft.mobility.CHRGUP.service.api.Data.DTOs.Vehicle;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.api.Models;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;

namespace esyasoft.mobility.CHRGUP.service.api.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly AppDbContext _db;

        public VehicleService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<VehicleResponseDto> CreateAsync(CreateVehicleDto dto)
        {
            if (await _db.vehicles.AnyAsync(v => v.VIN == dto.VIN))
                throw new InvalidOperationException("Vehicle with this VIN already exists");

            if (await _db.vehicles.AnyAsync(v => v.RegistrationNumber == dto.RegistrationNumber))
                throw new InvalidOperationException("Vehicle with this registration number already exists");

            var vehicle = new Vehicle
            {
                Id = Nanoid.Generate(size: 10),
                VehicleName = dto.VehicleName,
                VIN = dto.VIN,
                RegistrationNumber = dto.RegistrationNumber,
                MakeandModel = $"{dto.Make} {dto.Model} {dto.Variant}",
                RangeKm = dto.RangeKm,
                BatteryCapacityKwh = dto.BatteryCapacityKwh,
                MaxChargeRateKw = dto.MaxChargeRateKw
            };

            _db.vehicles.Add(vehicle);
            await _db.SaveChangesAsync();

            return await MapAsync(vehicle);
        }


        public async Task<List<VehicleResponseDto>> GetAllAsync()
        {
            var vehicles = await _db.vehicles.ToListAsync();
            var result = new List<VehicleResponseDto>();

            foreach (var v in vehicles)
                result.Add(await MapAsync(v));

            return result;
        }

        public async Task<VehicleResponseDto> GetByIdAsync(string vehicleId)
        {
            var vehicle = await _db.vehicles
                .FirstOrDefaultAsync(v => v.Id == vehicleId)
                ?? throw new KeyNotFoundException("Vehicle not found");

            return await MapAsync(vehicle);
        }

        public async Task DeleteAsync(string vehicleId)
        {
            var vehicle = await _db.vehicles
                .FirstOrDefaultAsync(v => v.Id == vehicleId)
                ?? throw new KeyNotFoundException("Vehicle not found");

            var isAssigned = await _db.drivers
                .AnyAsync(d => d.VehicleId == vehicleId);

            if (isAssigned)
                throw new InvalidOperationException("Vehicle is assigned to a driver");

            _db.vehicles.Remove(vehicle);
            await _db.SaveChangesAsync();
        }

        private async Task<VehicleResponseDto> MapAsync(Vehicle v)
        {
            var driver = await _db.drivers
                .FirstOrDefaultAsync(d => d.VehicleId == v.Id);

            return new VehicleResponseDto
            {
                VehicleId = v.Id,
                DriverId = driver?.Id,
                VehicleName = v.VehicleName,
                Make = ExtractMake(v.MakeandModel),
                Model = ExtractModel(v.MakeandModel),
                Variant = ExtractVariant(v.MakeandModel),
                RegistrationNumber = v.RegistrationNumber,
                VIN = v.VIN,
                RangeKm = v.RangeKm,
                BatteryCapacityKwh = v.BatteryCapacityKwh,
                MaxChargeRateKw = v.MaxChargeRateKw
            };
        }

        private static string ExtractMake(string value) =>
            value.Split(' ').FirstOrDefault() ?? "";

        private static string ExtractModel(string value) =>
            value.Split(' ').Skip(1).FirstOrDefault() ?? "";

        private static string ExtractVariant(string value) =>
            value.Split(' ').Skip(2).FirstOrDefault() ?? "";
    }
}
