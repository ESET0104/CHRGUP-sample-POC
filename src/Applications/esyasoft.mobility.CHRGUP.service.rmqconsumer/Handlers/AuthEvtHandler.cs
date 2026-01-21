using esyasoft.mobility.CHRGUP.service.core.Metadata;
using esyasoft.mobility.CHRGUP.service.core.Models;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using esyasoft.mobility.CHRGUP.service.rmqconsumer.DTOs;
using esyasoft.mobility.CHRGUP.service.rmqconsumer.Messaging;
using Microsoft.EntityFrameworkCore;

namespace esyasoft.mobility.CHRGUP.service.rmqconsumer.Handlers
{
    public class AuthEvtHandler
    {
        private readonly AppDbContext _db;
        private readonly ILogger _logger;
        private readonly RmqPublisher _publisher;
        public AuthEvtHandler(AppDbContext db, ILogger logger, RmqPublisher publisher) 
        {  
            _db = db; 
            _logger = logger;
            _publisher = publisher;
        }
        public async Task HandleAuthRequest(AuthReqEvent evt)
        {
            Driver? driver = null;

            switch (evt.TokenType)
            {
                case "VIN":
                    driver = await _db.drivers
                        .Include(d => d.Vehicle)
                        .FirstOrDefaultAsync(d =>
                            d.Vehicle != null &&
                            d.Vehicle.VIN == evt.Token);
                    break;

                case "RFID":
                    driver = await _db.drivers
                        .Include(d => d.Vehicle)
                        .FirstOrDefaultAsync(d =>
                            d.RfidTag == evt.Token);
                    break;

                default:
                    _logger.LogWarning(
                        "Unsupported token type {TokenType} for token {Token}",
                        evt.TokenType,
                        evt.Token);
                    break;
            }

            bool accepted =
                driver != null &&
                driver.Status == DriverStatus.Active;

            await _publisher.PublishAsync(
                "event.authorization.result",
                new
                {
                    evt.MessageId,
                    Accepted = accepted
                });

            _logger.LogInformation(
                "TOKEN {Token} ({Type}) auth = {Result}",
                evt.Token,
                evt.TokenType,
                accepted ? "ACCEPTED" : "REJECTED"
            );

            if (!accepted)
                return;

            var charger = await _db.chargers
                .FirstOrDefaultAsync(c => c.Id == evt.ChargerId);

            if (charger == null)
            {
                _logger.LogError(
                    "Charger {Id} not found for auth",
                    evt.ChargerId);
                return;
            }

            if (charger.Status == ChargerStatus.Available)
            {
                charger.Status = ChargerStatus.Preparing;
                charger.LastSeen = DateTime.UtcNow;

                await _db.SaveChangesAsync();

                _logger.LogInformation(
                    "Charger {Id} moved to PREPARING after auth",
                    charger.Id);
            }
        }

    }
}
