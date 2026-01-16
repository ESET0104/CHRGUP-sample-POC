using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
using System.Text.Json;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp16.Handlers
{
    public class StatusNotificationHandler
    {
        public static async Task Handle(JsonElement payload, string chargerId)
        {
            var connectorId = payload.GetProperty("connectorId").GetInt32();
            var status = payload.GetProperty("status").GetString();
            var errorCode = payload.TryGetProperty("errorCode", out var err)
                ? err.GetString()
                : null;

            // Treat status as heartbeat
            HeartbeatStore.Update(chargerId);

            if (status == "Faulted")
            {
                await RabbitMqEventPublisher.PublishAsync(
                    "event.charger.faulted",
                    new
                    {
                        ChargerId = chargerId,
                        ConnectorId = connectorId,
                        FaultCode = errorCode ?? "Unknown",
                        Timestamp = DateTime.UtcNow
                    });
            }
            else if (status == "Available")
            {
                await RabbitMqEventPublisher.PublishAsync(
                    "event.charger.recovered",
                    new
                    {
                        ChargerId = chargerId,
                        ConnectorId = connectorId,
                        Timestamp = DateTime.UtcNow
                    });
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
                        Timestamp = DateTime.UtcNow
                    });
            }
        }
    }
}
