using esyasoft.mobility.CHRGUP.service.core.Helpers;
using esyasoft.mobility.CHRGUP.service.ocpp.CanonicalEvents;
using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
using System.Text.Json;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp16.Handlers
{
    public class StatusNotificationHandler
    {
        public static async Task Handle(JsonElement payload, string chargerId)
        {
            var timestamp = DbTime.From(payload.GetProperty("timestamp").GetDateTime());
            var connectorId = payload.GetProperty("connectorId").GetInt32();
            var status = payload.GetProperty("status").GetString();
            var errorCode = payload.TryGetProperty("errorCode", out var err)
                ? err.GetString()
                : null;

            // Treat status as heartbeat
            HeartbeatStore.Update(chargerId);

            if (status == "Faulted")
            {

                var state = ChargerStateStore.Get(chargerId);
                state.IsFaulted = true;

                var ev = new ChargerFaultEvent
                {
                    ChargerId = chargerId,
                    FaultCode = errorCode ?? "Unknown",
                    Timestamp = timestamp
                };

                await RabbitMqEventPublisher.PublishAsync(
                    "event.charger.faulted",
                    ev);
                Console.WriteLine($"charger {chargerId} is being faulted here--v16");
            }
            else if (status == "Available")
            {

                var state = ChargerStateStore.Get(chargerId);
                state.IsFaulted = false;
                var ev = new ChargerRecoverEvent
                {
                    ChargerId = chargerId,
                    Timestamp = timestamp
                };
                await RabbitMqEventPublisher.PublishAsync(
                    "event.charger.recovered",
                   ev);
                Console.WriteLine($"charger {chargerId} is being recovered here--v16");
            }
            else
            {
                await RabbitMqEventPublisher.PublishAsync(
                    "event.connector.status",
                    new
                    {
                        ChargerId = chargerId,
                        ConnectorId = connectorId,
                        Status = status,
                        Timestamp = DbTime.From(DateTime.Now)
                    });
            }
        }
    }
}
