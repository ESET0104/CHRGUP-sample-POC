using esyasoft.mobility.CHRGUP.service.core.Helpers;
using esyasoft.mobility.CHRGUP.service.ocpp.CanonicalEvents;
using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
using esyasoft.mobility.CHRGUP.service.ocpp.WebSockets;
using System.Net.WebSockets;
using System.Text.Json;
using esyasoft.mobility.CHRGUP.service.ocpp.Ocpp201;
namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp16.Handlers
{
    public class TransactionHandler
    {

        public static async Task HandleStart(JsonElement payload, string chargerId, string messageId, WebSocket socket)
        {
            var rfid = payload.GetProperty("idTag").GetString();
            var soc = payload.TryGetProperty("soc", out var s) ? s.GetDouble() : 0;
            var timestamp = DbTime.From(payload.GetProperty("timestamp").GetDateTime());

            //var socket = ChargerConnectionManager.GetSocket(chargerId);
            //if (socket == null || socket.State != WebSocketState.Open)
            //    return;

            var state = ChargerStateStore.Get(chargerId);

            await OcppMessage.SendCallResult(socket, messageId, new
            {
                transactionId = state.ActiveSessionId,
                idTagInfo = new { status = "Accepted" }
            });

            var TransactionStart = new SessionStartEvent
            {
                ChargerId = chargerId,
                UserId = rfid!,
                StartTime = timestamp,
                SOC = soc
            };
            await RabbitMqEventPublisher.PublishAsync("event.session.started", TransactionStart);
            }

        public static async Task HandleStop(JsonElement payload, string chargerId, string messageId, WebSocket socket)
        {
            var triggerReason = payload.GetProperty("triggerReason").GetString();
            var sessionId = payload.GetProperty("transactionId").GetString();
            double energy = 0;
            if (payload.TryGetProperty("meterStop", out var meterStop))
            {
                energy = meterStop.GetDouble();
            }
            var soc = payload.TryGetProperty("soc", out var s)? s.GetDouble(): 0;
            var timestamp = DbTime.From(payload.GetProperty("timestamp").GetDateTime());

            //var socket = ChargerConnectionManager.GetSocket(chargerId);
            //if (socket == null || socket.State != WebSocketState.Open)
            //    return;

            var state = ChargerStateStore.Get(chargerId);

            await OcppMessage.SendCallResult(socket, messageId, new
            {
                idTagInfo = new { status = "Accepted" }
            });

            var TransactionStop = new SessionStopEvent
            {
                SessionId = sessionId,
                ChargerId = chargerId,
                StopTime = timestamp,
                EnergyConsumedKwh = energy,
                TriggerReason = triggerReason,
                SOC = soc
            };

            await RabbitMqEventPublisher.PublishAsync("event.session.stopped", TransactionStop);
        }

        
    }
}
