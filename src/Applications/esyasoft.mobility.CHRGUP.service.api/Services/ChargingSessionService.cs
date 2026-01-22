using esyasoft.mobility.CHRGUP.service.api.DTOs.ChargingSession;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using esyasoft.mobility.CHRGUP.service.core.Metadata;
using esyasoft.mobility.CHRGUP.service.core.Models;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;

namespace esyasoft.mobility.CHRGUP.service.api.Services
{
    public class ChargingSessionService : IChargingSessionService
    {
        private readonly AppDbContext _db;

        public ChargingSessionService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ChargingSessionResponseDto> StartAsync(StartChargingRequestDto dto)
        {
            var charger = await _db.chargers.FindAsync(dto.ChargerId)
                ?? throw new InvalidOperationException("Charger not found");

            var driver = await _db.drivers.FindAsync(dto.DriverId)
                ?? throw new InvalidOperationException("Driver not found");

            var activeSessionExists = await _db.chargingSessions
                .AnyAsync(s => s.ChargerId == dto.ChargerId &&
                               s.Status != SessionStatus.Completed);

            if (activeSessionExists)
                throw new InvalidOperationException("Charger already in use");

            var session = new ChargingSession
            {
                Id = Nanoid.Generate(size: 10),
                ChargerId = dto.ChargerId,
                DriverId = dto.DriverId,
                Status = SessionStatus.Pending,
                StartTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified)
            };

            _db.chargingSessions.Add(session);
            await _db.SaveChangesAsync();

            return Map(session);
        }

        public async Task StopAsync(string sessionId)
        {
            var session = await _db.chargingSessions.FindAsync(sessionId)
                ?? throw new InvalidOperationException("Session not found");

            if (session.Status == SessionStatus.Completed)
                throw new InvalidOperationException("Session already completed");

            session.Status = SessionStatus.Stopping;
            await _db.SaveChangesAsync();
        }


        public async Task<ChargingSessionResponseDto> GetByIdAsync(string sessionId)
        {
            var session = await _db.chargingSessions.FindAsync(sessionId)
                ?? throw new InvalidOperationException("Session not found");

            return Map(session);
        }

        private static ChargingSessionResponseDto Map(ChargingSession s)
        {
            return new ChargingSessionResponseDto
            {
                SessionId = s.Id,
                ChargerId = s.ChargerId,
                DriverId = s.DriverId,
                Status = s.Status.ToString(),
                StartTime = s.StartTime,
                EndTime = s.EndTime
            };
        }
    }
}
