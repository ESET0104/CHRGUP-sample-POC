using esyasoft.mobility.CHRGUP.service.api.DTOs.Fault;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace esyasoft.mobility.CHRGUP.service.api.Services
{
    public class FaultService : IFaultService
    {
        private readonly AppDbContext _db;

        public FaultService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<FaultResponseDto>> GetAllAsync()
        {
            return await _db.faults
                .OrderByDescending(f => f.Timestamp)
                .Select(f => Map(f))
                .ToListAsync();
        }

        public async Task<List<FaultResponseDto>> GetByChargerIdAsync(string chargerId)
        {
            return await _db.faults
                .Where(f => f.ChargerId == chargerId)
                .OrderByDescending(f => f.Timestamp)
                .Select(f => Map(f))
                .ToListAsync();
        }

        public async Task<FaultResponseDto> GetByIdAsync(string id)
        {
            var fault = await _db.faults
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fault == null)
                throw new KeyNotFoundException("Fault not found");

            return Map(fault);
        }

        private static FaultResponseDto Map(core.Models.Fault f)
        {
            return new FaultResponseDto
            {
                Id = f.Id,
                ChargerId = f.ChargerId,
                FaultCode = f.FaultCode,
                Timestamp = f.Timestamp,
                Severity = f.Severity.ToString(),
            };
        }
    }
}
