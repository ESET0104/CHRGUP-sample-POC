using esyasoft.mobility.CHRGUP.service.api.Infrastructure.Messaging;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.core.Helpers;
using esyasoft.mobility.CHRGUP.service.core.Metadata;
using esyasoft.mobility.CHRGUP.service.core.Models;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;

namespace esyasoft.mobility.CHRGUP.service.api.Services
{
    public class ChargerService : IChargerService
    {
        private readonly AppDbContext _db;
        private readonly RmqPublisher _publisher;
        private static readonly Random random = new Random();

        public ChargerService(AppDbContext db, RmqPublisher publisher)
        {
            _db = db;
            _publisher = publisher;
        }

        public async Task<List<Charger>> GetAllAsync()
        {
            return await _db.chargers.ToListAsync();
        }

        public async Task<Charger> RegisterAsync(string locationId, string version)
        {
            var locationExists = await _db.locations
        .AnyAsync(l => l.Id == locationId);

            if (!locationExists)
                throw new ArgumentException("Invalid LocationId");

            var config = new ChargerConfig
            {
                ChargerId = Nanoid.Generate(size: 10),
                Manufacturer = "esyasoft",
                FirmwareVersion = version,
                InputPower = 10.0 + (random.NextDouble() * (20)),
                OutputPower = 15.0 + random.NextDouble() * 10,
                ConnectorType = "Type-1",
                NoOfPorts = 1
            };

            var charger = new Charger
            {
                Id = config.ChargerId,
                Status = ChargerStatus.Available,
                LastSeen = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified),
                LocationId = locationId
            };

            _db.chargerConfigs.Add(config);
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

            charger.LastSeen = DbTime.From(timestamp);
            await _db.SaveChangesAsync();
        }

        //public async Task RemoteStartAsync(string chargerId, StartChargingRequestDto dto)
        //{
        //    if (!await _db.chargers.AnyAsync(c => c.Id == chargerId))
        //        throw new InvalidOperationException("Charger not found");

        //    var command = new RemoteStartCommand
        //    {
        //        ChargerId = chargerId,
        //        //ConnectorId = dto.ConnectorId,
        //        DriverId = dto.DriverId,
        //        RequestedAt = DateTime.Now
        //    };

        //    await _publisher.PublishAsync(command);
        //}

        //public async Task RemoteStopAsync(string chargerId, StopChargingRequestDto dto)
        //{
        //    var command = new RemoteStopCommand
        //    {
        //        ChargerId = chargerId,
        //        SessionId = dto.SessionId,
        //        RequestedAt = DateTime.Now
        //    };

        //    await _publisher.PublishAsync(command);
        //}
    }
}
