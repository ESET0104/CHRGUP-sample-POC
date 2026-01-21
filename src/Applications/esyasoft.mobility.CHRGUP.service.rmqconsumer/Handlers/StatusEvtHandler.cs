using esyasoft.mobility.CHRGUP.service.core.Metadata;
using esyasoft.mobility.CHRGUP.service.core.Models;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using esyasoft.mobility.CHRGUP.service.rmqconsumer.DTOs;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

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
            var fault = new Fault
            {
                Id = Nanoid.Generate(size: 10),
                ChargerId = evt.ChargerId,
                FaultCode = evt.FaultCode,
                Timestamp = evt.Timestamp
            };

            _db.faults.Add(fault);

            var charger = await _db.chargers
                .FirstOrDefaultAsync(c => c.Id == evt.ChargerId);

            if (charger != null)
            {
                charger.Status = ChargerStatus.Faulted;
                charger.LastSeen = DateTime.UtcNow;
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
                activeSession.EndTime = evt.Timestamp;
            }

            await _logger.SaveLogAsync(
                source: "ocpp",
                eventType: "CHARGER_FAULTED",
                message: evt.FaultCode,
                chargerId: evt.ChargerId,
                sessionId: activeSession?.Id,
                driverId: activeSession?.DriverId
            );

            await _db.SaveChangesAsync();
        }


        public async Task HandleChargerRecovered(ChargerRecoverEvent evt)
        {
            var charger = await _db.chargers
                .FirstOrDefaultAsync(c => c.Id == evt.ChargerId);

            if (charger != null)
            {
                charger.Status = ChargerStatus.Available;
                charger.LastSeen = evt.Timestamp;
            }

            await _logger.SaveLogAsync(
                source: "ocpp",
                eventType: "CHARGER_RECOVERED",
                message: "Charger recovered",
                chargerId: evt.ChargerId
            );

            await _db.SaveChangesAsync();
        }
    }
}
