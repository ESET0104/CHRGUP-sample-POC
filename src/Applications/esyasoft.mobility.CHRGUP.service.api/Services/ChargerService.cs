using esyasoft.mobility.CHRGUP.service.api.Data;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.api.Metadata;
using esyasoft.mobility.CHRGUP.service.api.Models;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;

namespace esyasoft.mobility.CHRGUP.service.api.Services
{
    public class ChargerService : IChargerService
    {
        private readonly AppDbContext _db;

        public ChargerService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Charger>> GetAllAsync()
        {
            return await _db.chargers.ToListAsync();
        }

        public async Task<Charger> RegisterAsync(string locationId)
        {
            var locationExists = await _db.locations
        .AnyAsync(l => l.Id == locationId);

            if (!locationExists)
                throw new ArgumentException("Invalid LocationId");

            var charger = new Charger
            {
                Id = Nanoid.Generate(size: 10),
                Status = ChargerStatus.Available,
                LastSeen = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                LocationId = locationId
            };

            _db.chargers.Add(charger);
            await _db.SaveChangesAsync();

            return charger;
        }

        public async Task UpdateStatusAsync(string chargerId, ChargerStatus status)
        {
            var charger = await _db.chargers
                .FirstOrDefaultAsync(c => c.Id == chargerId)
                ?? throw new KeyNotFoundException("Charger not found");

            charger.Status = status;
            await _db.SaveChangesAsync();
        }

        public async Task UpdateHeartbeatAsync(string chargerId, DateTime timestamp)
        {
            var charger = await _db.chargers
                .FirstOrDefaultAsync(c => c.Id == chargerId)
                ?? throw new KeyNotFoundException("Charger not found");

            charger.LastSeen = timestamp;
            await _db.SaveChangesAsync();
        }
    }
}
