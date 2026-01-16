using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
using System.Text.Json;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp16.Handlers
{
    public class TransactionHandler
    {
        public static async Task HandleStart(JsonElement payload, string chargerId)
        {
            var rfid = payload.GetProperty("idTag").GetString();
            var txId = payload.GetProperty("transactionId").GetInt32().ToString();

            var session = CanonicalSessionStore.GetOrCreate(chargerId, OcppProtocol.V16,1);
            session.SessionId = txId;
            session.UserId = rfid;
            session.Active = true;

            if (payload.TryGetProperty("meterStart", out var meterStart))
            {
                session.EnergyKwh = meterStart.GetDouble();
            }

            await RabbitMqEventPublisher.PublishAsync(
                "event.session.started",
                new { ChargerId = chargerId, SessionId = txId, EnergyKwh = session.EnergyKwh, Soc = session.Soc });
        }

        public static async Task HandleStop(JsonElement payload, string chargerId)
        {
            var session = CanonicalSessionStore.GetOrCreate(chargerId, OcppProtocol.V16,1);
            session.Active = false;

            if (payload.TryGetProperty("meterStop", out var meterStop))
            {
                session.EnergyKwh = meterStop.GetDouble();
            }

            session.Active = false;

            await RabbitMqEventPublisher.PublishAsync(
                "event.session.stopped",
                new { ChargerId = chargerId, SessionId = session.SessionId , EnergyKwh = session.EnergyKwh, Soc = session.Soc, EvseId =1});

            CanonicalSessionStore.Remove(chargerId, 1);
        }
    }
}
