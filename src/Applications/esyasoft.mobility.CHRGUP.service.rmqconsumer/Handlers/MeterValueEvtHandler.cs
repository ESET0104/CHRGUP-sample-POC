using esyasoft.mobility.CHRGUP.service.core.Helpers;
using esyasoft.mobility.CHRGUP.service.core.Metadata;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using esyasoft.mobility.CHRGUP.service.rmqconsumer.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace esyasoft.mobility.CHRGUP.service.rmqconsumer.Handlers
{
    public class MeterValueEvtHandler
    {
        private readonly AppDbContext _db;
        private readonly ILogger<MeterValueEvtHandler> _logger;
        private readonly AuditLogger _auditlogger;
        public MeterValueEvtHandler(AppDbContext db, ILogger<MeterValueEvtHandler> logger, AuditLogger auditlogger)
        {
            _db = db;
            _logger = logger;
            _auditlogger = auditlogger;
        }

        public async Task HandleMeterValue(MeterValueEvent evt)
        {
            var session = await _db.chargingSessions
                .FirstOrDefaultAsync(s => s.Id == evt.SessionId);

            if (session == null ||
                session.Status != SessionStatus.Active ||
                session.ChargerId != evt.ChargerId)
                return;

            if (session.LastMeterUpdate != null &&
                evt.Timestamp <= session.LastMeterUpdate)
                return;

            session.LastMeterUpdate = evt.Timestamp;
            session.EnergyConsumedKwh = evt.EnergyKwh;
            session.SOC = evt.SOC;

            var log = await _auditlogger.SaveLogAsync(
                source: "charger",
                eventType: "METER_VALUE",
                message: $"Energy consumed updated: {evt.EnergyKwh} kWh",
                chargerId: evt.ChargerId,
                sessionId: evt.SessionId,
                driverId: session.DriverId
            );

            _db.logs.Add( log );

            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "MeterValue received: Session={SessionId}, Energy={Energy} kWh",
                evt.SessionId,
                evt.EnergyKwh
            );
        }
    }
}
