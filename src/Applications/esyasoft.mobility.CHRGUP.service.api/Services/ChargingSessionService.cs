using esyasoft.mobility.CHRGUP.service.api.DTOs.Session;
//using esyasoft.mobility.CHRGUP.service.api.DTOs.Charger;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using esyasoft.mobility.CHRGUP.service.core.Metadata;
using esyasoft.mobility.CHRGUP.service.core.Models;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;
using esyasoft.mobility.CHRGUP.service.core.Helpers;
using esyasoft.mobility.CHRGUP.service.api.DTOs.Messaging;
using esyasoft.mobility.CHRGUP.service.api.Infrastructure.Messaging;

namespace esyasoft.mobility.CHRGUP.service.api.Services
{
    public class ChargingSessionService : IChargingSessionService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<ChargingSessionService> _logger;
        private readonly AuditLogger _auditLogger;
        private readonly RmqPublisher _publisher;

        public ChargingSessionService(AppDbContext db, ILogger<ChargingSessionService> logger, AuditLogger auditLogger, RmqPublisher publisher)
        {
            _db = db;
            _logger = logger;
            _auditLogger = auditLogger;
            _publisher = publisher;
        }

        //public async Task<ChargingSessionResponseDto> StartAsync(StartChargingRequestDto dto)
        //{
        //    var charger = await _db.chargers.FindAsync(dto.ChargerId)
        //        ?? throw new InvalidOperationException("Charger not found");

        //    var driver = await _db.drivers.FindAsync(dto.DriverId)
        //        ?? throw new InvalidOperationException("Driver not found");

        //    var activeSessionExists = await _db.chargingSessions
        //        .AnyAsync(s => s.ChargerId == dto.ChargerId &&
        //                       s.Status != SessionStatus.Completed);

        //    if (activeSessionExists)
        //        throw new InvalidOperationException("Charger already in use");

        //    var session = new ChargingSession
        //    {
        //        Id = Nanoid.Generate(size: 10),
        //        ChargerId = dto.ChargerId,
        //        DriverId = dto.DriverId,
        //        Status = SessionStatus.Pending,
        //        StartTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified)
        //    };

        //    _db.chargingSessions.Add(session);
        //    await _db.SaveChangesAsync();

        //    return Map(session);
        //}

        //public async Task StopAsync(string sessionId)
        //{
        //    var session = await _db.chargingSessions.FindAsync(sessionId)
        //        ?? throw new InvalidOperationException("Session not found");

        //    if (session.Status == SessionStatus.Completed)
        //        throw new InvalidOperationException("Session already completed");

        //    session.Status = SessionStatus.Stopping;
        //    await _db.SaveChangesAsync();
        //}


        public async Task<ChargingSessionResponseDto> GetByIdAsync(string sessionId)
        {
            var session = await _db.chargingSessions.FindAsync(sessionId)
                ?? throw new InvalidOperationException("Session not found");

            return Map(session);
        }

        public async Task<List<ChargingSessionResponseDto>> GetByChargerAsync(string chargerId)
        {
            return await _db.chargingSessions
                .Where(s => s.ChargerId == chargerId)
                .OrderByDescending(s => s.StartTime)
                .Select(s => Map(s))
                .ToListAsync();
        }

        public async Task<string> StartSessionAsync(
            string chargerId,
            string driverId)
        {
            var charger = await _db.chargers
                .FirstOrDefaultAsync(c => c.Id == chargerId);

            if (charger == null)
                throw new Exception("charger not found");

            if (charger.Status != ChargerStatus.Preparing)
                throw new Exception("vehicle not authorized or charger not ready");

            var driver = await _db.drivers
                .FirstOrDefaultAsync(d => d.Id == driverId);

            if (driver == null)
                throw new Exception("Driver not authorized");

            var existingSession = await _db.chargingSessions
                .FirstOrDefaultAsync(s =>
                    s.ChargerId == charger.Id &&
                    s.Status == SessionStatus.Active);

            if (existingSession != null)
                throw new Exception("Charger already in use");

            _logger.LogInformation(
                "Starting session for Charger={ChargerId}, DriverId={DriverId}",
                charger.Id,
                driver.Id
            );

            var session = new ChargingSession
            {
                Id = Nanoid.Generate(size: 10),
                ChargerId = charger.Id,
                DriverId = driver.Id,
                Status = SessionStatus.Pending
            };

            _db.chargingSessions.Add(session);
            charger.Status = ChargerStatus.Preparing;

            var log = await _auditLogger.SaveLogAsync(
                source: "backend-api",
                eventType: "SESSION_REQUESTED",
                message: "Start charging session requested",
                chargerId: charger.Id,
                sessionId: session.Id,
                driverId: driver.Id
            );

            _db.logs.Add( log );

            await _db.SaveChangesAsync();

            var command = new RemoteStartCommand
            {
                ChargerId = chargerId,
                SessionId = session.Id,
                DriverId = driverId,
                Timestamp = DbTime.From(DateTime.Now)
            };

            await _publisher.PublishAsync("command.start", command);

            return session.Id;
        }

        public async Task StopSessionAsync(string sessionId)
        {
            var session = await _db.chargingSessions
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null)
                throw new Exception("Session not found");

            if (session.Status != SessionStatus.Active)
                throw new Exception("Session not active");

            session.Status = SessionStatus.Stopping;

            var log = await _auditLogger.SaveLogAsync(
                source: "backend-api",
                eventType: "SESSION_STOP_REQUESTED",
                message: "Stop charging session requested",
                chargerId: session.ChargerId,
                sessionId: session.Id,
                driverId: session.DriverId
            );
            _db.logs.Add(log);

            await _db.SaveChangesAsync();
            
            var command = new RemoteStopCommand
            {
                ChargerId = session.ChargerId,
                SessionId = sessionId,
                Timestamp = DbTime.From(DateTime.Now)
            };

            await _publisher.PublishAsync("command.stop", command);
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
