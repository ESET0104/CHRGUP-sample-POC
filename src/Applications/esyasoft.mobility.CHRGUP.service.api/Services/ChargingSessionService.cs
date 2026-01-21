//using esyasoft.mobility.CHRGUP.service.api.DTOs.Charger;
//using esyasoft.mobility.CHRGUP.service.api.DTOs.Session;
//using esyasoft.mobility.CHRGUP.service.api.Interfaces;
//using esyasoft.mobility.CHRGUP.service.core.Metadata;
//using esyasoft.mobility.CHRGUP.service.core.Models;
//using esyasoft.mobility.CHRGUP.service.persistence.Data;
//using Microsoft.EntityFrameworkCore;
//using NanoidDotNet;

//namespace esyasoft.mobility.CHRGUP.service.api.Services
//{

//     public class ChargingSessionService : IChargingSessionService
//     {
//         private readonly AppDbContext _db;

//         public ChargingSessionService(AppDbContext db)
//         {
//             _db = db;
//         }

//         public async Task<ChargingSessionResponseDto> StartAsync(StartChargingRequestDto dto)
//         {
//             var charger = await _db.chargers.FindAsync(dto.ChargerId)
//                 ?? throw new InvalidOperationException("Charger not found");

//             var driver = await _db.drivers.FindAsync(dto.DriverId)
//                 ?? throw new InvalidOperationException("Driver not found");

//             var activeSessionExists = await _db.chargingSessions
//                 .AnyAsync(s => s.ChargerId == dto.ChargerId &&
//                                s.Status != SessionStatus.Completed);

//             if (activeSessionExists)
//                 throw new InvalidOperationException("Charger already in use");

//             var session = new ChargingSession
//             {
//                 Id = Nanoid.Generate(size: 10),
//                 ChargerId = dto.ChargerId,
//                 DriverId = dto.DriverId,
//                 Status = SessionStatus.Pending,
//                 StartTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
//             };

//             _db.chargingSessions.Add(session);
//             await _db.SaveChangesAsync();

//             return Map(session);
//         }

//         public async Task StopAsync(string sessionId)
//         {
//             var session = await _db.chargingSessions.FindAsync(sessionId)
//                 ?? throw new InvalidOperationException("Session not found");

//             if (session.Status == SessionStatus.Completed)
//                 throw new InvalidOperationException("Session already completed");

//             session.Status = SessionStatus.Stopping;
//             await _db.SaveChangesAsync();
//         }


//         public async Task<ChargingSessionResponseDto> GetByIdAsync(string sessionId)
//         {
//             var session = await _db.chargingSessions.FindAsync(sessionId)
//                 ?? throw new InvalidOperationException("Session not found");

//             return Map(session);
//         }

//         private static ChargingSessionResponseDto Map(ChargingSession s)
//         {
//             return new ChargingSessionResponseDto
//             {
//                 SessionId = s.Id,
//                 ChargerId = s.ChargerId,
//                 DriverId = s.DriverId,
//                 Status = s.Status.ToString(),
//                 StartTime = s.StartTime,
//                 EndTime = s.EndTime
//             };
//         }
//     }

//}


using esyasoft.mobility.CHRGUP.service.api.DTOs.Session;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace esyasoft.mobility.CHRGUP.service.api.Services
{
    public class ChargingSessionService : IChargingSessionService
    {
        private readonly AppDbContext _db;

        public ChargingSessionService(AppDbContext db)
        {
            _db = db;
        }

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

        private static ChargingSessionResponseDto Map(core.Models.ChargingSession s)
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
