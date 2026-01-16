//using System.Text.Json;
//using OcppMicroservice.Messaging;
//namespace OcppMicroservice.Ocpp.Handlers
//{
//    public static class StatusNotificationHandler
//    {
//        public static void Handle(
//            JsonElement payload,
//            string chargePointId)
//        {
//            var connectorId = payload.GetProperty("connectorId").GetInt32();
//            var status = payload.GetProperty("connectorStatus").GetString();

//            RabbitMqEventPublisher.PublishAsync("connector.status", new
//            {
//                chargePointId,
//                connectorId,
//                status,
//                timestamp = DateTime.UtcNow
//            });
//        }
//    }
//}

using System.Text.Json;
using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.State;

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


            var connectorId = payload.GetProperty("connectorId").GetInt32();
            var status = payload.GetProperty("connectorStatus").GetString();
            var timestamp = payload.GetProperty("timestamp").GetDateTime();

            string? errorCode = null;
            if (payload.TryGetProperty("errorCode", out var err))
            {
                errorCode = err.GetString();
            }

            var state = ChargerStateStore.Get(chargePointId);

            if (status == "Faulted")
            {
                state.IsFaulted = true;
                await RabbitMqEventPublisher.PublishAsync(
                    "event.charger.faulted",
                    new
                    {
                        ChargerId = chargePointId,
                        ConnectorId = connectorId,
                        FaultCode = errorCode ?? "Unknown",
                        Timestamp = timestamp
                    }
                );

                //if (state.ActiveSessionId != null)
                //{
                //    state.ActiveSessionId = null;
                //}
                var session = CanonicalSessionStore.GetOrCreate(chargePointId, OcppProtocol.V201, evseId);
                session.Active = false;
            }
            if (status == "Available")
            {
                state.IsFaulted = false;
                HeartbeatStore.Update(chargePointId);
                await RabbitMqEventPublisher.PublishAsync(
                    "event.charger.recovered",
                    new
                    {
                        ChargerId = chargePointId,
                        Timestamp = timestamp
                    }
                );
            }
        }
    }
}

