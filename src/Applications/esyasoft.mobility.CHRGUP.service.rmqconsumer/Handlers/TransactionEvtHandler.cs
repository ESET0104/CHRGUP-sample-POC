using esyasoft.mobility.CHRGUP.service.core.Metadata;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using esyasoft.mobility.CHRGUP.service.rmqconsumer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace esyasoft.mobility.CHRGUP.service.rmqconsumer.Handlers
{
    public class TransactionEvtHandler
    {
        private readonly AppDbContext _db;
        private readonly ILogger _logger;
        private readonly AuditLogger _logger1;
        public TransactionEvtHandler(AppDbContext db, ILogger logger, AuditLogger logger1)
        {
            _db = db;
            _logger = logger;
            _logger1 = logger1;
        }
        public async Task HandleSessionStarted(SessionStartEvent evt)
        {
            var session = await _db.chargingSessions
                .FirstOrDefaultAsync(s => s.Id == evt.SessionId);

            if (session == null || session.Status != SessionStatus.Pending)
                return;

            if (session.ChargerId != evt.ChargerId)
            {
                session.Status = SessionStatus.Faulted;
                await _db.SaveChangesAsync();
                return;
            }

            session.Status = SessionStatus.Active;
            session.StartTime = evt.StartTime;
            session.InitialCharge = evt.SOC;
            session.SOC = evt.SOC;


            var charger = await _db.chargers
                .FirstOrDefaultAsync(c => c.Id == evt.ChargerId);

            if (charger != null)
                charger.Status = ChargerStatus.Engaged;

            await _logger1.SaveLogAsync(
                source: "charger",
                eventType: "SESSION_STARTED",
                message: "Charging session started",
                chargerId: evt.ChargerId,
                sessionId: evt.SessionId,
                driverId: session.DriverId
            );

            await _db.SaveChangesAsync();
        }

        public async Task HandleSessionStopped(SessionStopEvent evt)
        {
            var session = await _db.chargingSessions
                .FirstOrDefaultAsync(s => s.Id == evt.SessionId);

            if (session == null ||
                session.Status == SessionStatus.Completed ||
                session.ChargerId != evt.ChargerId)
                return;

            if (evt.triggerReason == "Fault")
            {
                _logger.LogWarning(
                    "Stop event due to fault for Session={SessionId}",
                    evt.SessionId
                );
                return;
            }

            session.Status = SessionStatus.Completed;
            session.EndTime = evt.StopTime;
            session.SOC = evt.SOC;

            var charger = await _db.chargers
                .FirstOrDefaultAsync(c => c.Id == evt.ChargerId);

            if (charger != null)
                charger.Status = ChargerStatus.Available;

            await _logger1.SaveLogAsync(
                source: "charger",
                eventType: "SESSION_ENDED",
                message: "Charging session completed",
                chargerId: evt.ChargerId,
                sessionId: evt.SessionId,
                driverId: session.DriverId
            );

            await _db.SaveChangesAsync();
        }
    }
}
