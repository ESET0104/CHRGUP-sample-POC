using esyasoft.mobility.CHRGUP.service.ocpp.CanonicalEvents;
using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
using System.Text.Json;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp201.Handlers
{
    public static class StatusNotificationHandler
    {
        public static async Task Handle(
            JsonElement payload,
            string chargePointId)
        {
            HeartbeatStore.Update(chargePointId);

            var evseId = payload.GetProperty("evseId").GetProperty("id").GetInt32();
            //var connectorId = payload.GetProperty("connectorId").GetInt32();
            var status = payload.GetProperty("connectorStatus").GetString();
            var timestamp = payload.GetProperty("timestamp").GetDateTime();

            string? errorCode = null;
            if (payload.TryGetProperty("errorCode", out var err))
            {
                errorCode = err.GetString();
            }

            var state = ChargerStateStore.Get(chargePointId);
            state.LastSeenUtc = timestamp;

            if (status == "Faulted")
            {
                state.IsFaulted = true;
                var ev = new ChargerFaultEvent
                {
                    ChargerId = chargePointId,
                    FaultCode = errorCode ?? "Unknown",
                    Timestamp = timestamp
                };
                await RabbitMqEventPublisher.PublishAsync(
                    "event.charger.faulted",
                    ev
                );
            }
            if (status == "Available")
            {
                state.IsFaulted = false;
                HeartbeatStore.Update(chargePointId);
                var ev = new ChargerRecoverEvent
                {
                    ChargerId = chargePointId,
                    Timestamp = timestamp
                };
                await RabbitMqEventPublisher.PublishAsync(
                    "event.charger.recovered",ev
                );
            }
        }
    }
}

