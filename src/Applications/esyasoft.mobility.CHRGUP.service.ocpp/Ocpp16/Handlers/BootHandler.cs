using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
using esyasoft.mobility.CHRGUP.service.ocpp.WebSockets;
using System.Data;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp16.Handlers
{
    public class BootHandler
    {
        public static async Task Handle(
            string chargerId,
            string messageId,
            WebSocket socket)
        {
            HeartbeatStore.Update(chargerId);
            var conn = ChargerConnectionManager.GetConnection(chargerId);
            conn?.LockProtocol(LockedOcppProtocol.Ocpp16);
            ChargerProtocolStore.Set(chargerId, OcppProtocol.V16);
            await RabbitMqEventPublisher.PublishAsync(
                "event.charger.connected",
                new
                {
                    ChargerId = chargerId,
                    Protocol = "1.6",
                    Timestamp = DateTime.UtcNow
                });

            var response = new object[]
            {
                3,
                messageId,
                new
                {
                    status = "Accepted",
                    currentTime = DateTime.UtcNow,
                    heartbeatInterval = 10
                }
            };
            ChargerProtocolStore.MarkBooted(chargerId);

            var json = JsonSerializer.Serialize(response);
            var bytes = Encoding.UTF8.GetBytes(json);

            await socket.SendAsync(
                bytes,
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }
}
