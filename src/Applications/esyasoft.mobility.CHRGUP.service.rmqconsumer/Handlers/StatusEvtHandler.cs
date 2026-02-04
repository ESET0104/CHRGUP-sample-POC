using esyasoft.mobility.CHRGUP.service.core.Helpers;
using esyasoft.mobility.CHRGUP.service.core.Metadata;
using esyasoft.mobility.CHRGUP.service.core.Models;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using esyasoft.mobility.CHRGUP.service.rmqconsumer.DTOs;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;

namespace esyasoft.mobility.CHRGUP.service.rmqconsumer.Handlers
{
    public class StatusEvtHandler
    {
        private readonly AppDbContext _db;
        private readonly AuditLogger _logger;
        public StatusEvtHandler(AppDbContext db, AuditLogger logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task HandleChargerFault(ChargerFaultEvent evt)
        {
            

            var charger = await _db.chargers
                .FirstOrDefaultAsync(c => c.Id == evt.ChargerId);

            if (charger != null)
            {
                charger.Status = ChargerStatus.Faulted;
                charger.LastSeen = DateTime.Now;

                var fault = new Fault
                {
                    Id = Nanoid.Generate(size: 10),
                    ChargerId = evt.ChargerId,
                    FaultCode = evt.FaultCode,
                    Severity = evt.Severity,
                    Timestamp = DbTime.From(evt.Timestamp)
                };

                _db.faults.Add(fault);
            }

            var activeSession = await _db.chargingSessions
                .FirstOrDefaultAsync(s =>
                    s.ChargerId == evt.ChargerId &&
                    (s.Status == SessionStatus.Active ||
                     s.Status == SessionStatus.Pending ||
                     s.Status == SessionStatus.Stopping));

            if (activeSession != null)
            {
                activeSession.Status = SessionStatus.Faulted;
                activeSession.EndTime = DbTime.From(evt.Timestamp);
            }

            

            Console.WriteLine($"charger {evt.ChargerId} is faulted");

            var log = await _logger.SaveLogAsync(
                source: "ocpp",
                eventType: "CHARGER_FAULTED",
                message: evt.FaultCode,
                chargerId: evt.ChargerId,
                sessionId: activeSession?.Id,
                driverId: activeSession?.DriverId
            );
            _db.logs.Add(log);

            await _db.SaveChangesAsync();
        }


        public async Task HandleChargerRecovered(ChargerRecoverEvent evt)
        {
            var charger = await _db.chargers
                .FirstOrDefaultAsync(c => c.Id == evt.ChargerId);
            

            if (charger != null)
            {
                Console.WriteLine($"charger {evt.ChargerId} is found");
                Console.WriteLine($"DB Status before update: {charger.Status}");
                charger.Status = ChargerStatus.Available;
                charger.LastSeen = DbTime.From(evt.Timestamp);
                Console.WriteLine($"{charger.Status}");
            }
            else
            {
                Console.WriteLine($"charger {evt.ChargerId} is not found");
            }
            Console.WriteLine($"entered charger {evt.ChargerId} recovery handler");

            var log = await _logger.SaveLogAsync(
                source: "ocpp",
                eventType: "CHARGER_RECOVERED",
                message: "Charger recovered",
                chargerId: evt.ChargerId
            );
            _db.logs.Add(log);

            await _db.SaveChangesAsync();
        }
    }
}
