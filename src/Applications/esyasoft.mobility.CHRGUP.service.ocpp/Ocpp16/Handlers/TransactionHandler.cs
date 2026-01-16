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

            var session = CanonicalSessionStore.GetOrCreate(chargerId, OcppProtocol.V16);
            session.SessionId = txId;
            session.UserId = rfid;
            session.Active = true;

            await RabbitMqEventPublisher.PublishAsync(
                "event.session.started",
                new { ChargerId = chargerId, SessionId = txId });
        }

        public static async Task HandleStop(JsonElement payload, string chargerId)
        {
            var session = CanonicalSessionStore.GetOrCreate(chargerId, OcppProtocol.V16);
            session.Active = false;

            await RabbitMqEventPublisher.PublishAsync(
                "event.session.stopped",
                new { ChargerId = chargerId, SessionId = session.SessionId });
        }
    }
}
