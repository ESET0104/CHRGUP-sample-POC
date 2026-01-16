using System.Text.Json;
using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.State;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp201.Handlers
{
    public static class TransactionEventHandler
    {
        

        public static async Task Handle(
            JsonElement payload,
            string chargePointId)
        {
            HeartbeatStore.Update(chargePointId);
            var eventType = payload.GetProperty("eventType").GetString();
            var timestamp = payload.GetProperty("timestamp").GetDateTime();
            var triggerReason = payload.GetProperty("triggerReason").GetString();

            var transactionInfo = payload.GetProperty("transactionInfo");
            var sessionId = transactionInfo
                .GetProperty("sessionId")
                .GetString();

            //
            var evseId = payload.GetProperty("evse").GetProperty("id").GetInt32();

            //


            //var state = ChargerStateStore.Get(chargePointId);
            var session = CanonicalSessionStore.GetOrCreate(chargePointId, OcppProtocol.V201, evseId);

            if (eventType == "Started")
            {
                //state.ActiveSessionId = sessionId;
                session.SessionId = sessionId;
                session.Active = true;


                await RabbitMqEventPublisher.PublishAsync(
                    "event.session.started",
                    new
                    {
                        SessionId = sessionId,
                        ChargerId = chargePointId,
                        StartTime = timestamp
                    });
            }
            else if (eventType == "Ended")
            {
                Console.WriteLine("stop event received at ocpp");

                await RabbitMqEventPublisher.PublishAsync(
                    "event.session.stopped",
                    new
                    {
                        SessionId = sessionId,
                        ChargerId = chargePointId,
                        StopTime = timestamp,
                        triggerReason
                    });


                session.Active = false;
                //state.ActiveSessionId = null;
            }
        }
    }
}

