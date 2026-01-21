using esyasoft.mobility.CHRGUP.service.core.Models;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace esyasoft.mobility.CHRGUP.service.rmqconsumer.Handlers
{
    public class AuditLogger
    {
        private readonly AppDbContext _db;
        public AuditLogger(AppDbContext db)
        {
            _db = db;
        }

        public async Task SaveLogAsync(
            string source,
            string eventType,
            string message,
            string? chargerId = null,
            string? sessionId = null,
            string? driverId = null)
        {
            var log = new Log
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                Source = source,
                EventType = eventType,
                Message = message,
                ChargerId = chargerId,
                SessionId = sessionId,
                DriverId = driverId
            };

            _db.logs.Add(log);
        }
    }
}
