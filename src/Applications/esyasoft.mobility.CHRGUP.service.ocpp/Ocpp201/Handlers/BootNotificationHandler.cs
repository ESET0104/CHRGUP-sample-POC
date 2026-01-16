using esyasoft.mobility.CHRGUP.service.ocpp.State;
using esyasoft.mobility.CHRGUP.service.ocpp.WebSockets;
using System.Data;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp201.Handlers
{
    
    public static class BootNotificationHandler
    {
        public static async Task Handle(
            string chargePointId,
            string messageId,
            WebSocket socket)
        {
            HeartbeatStore.Update(chargePointId);
            ChargerProtocolStore.Set(chargePointId, OcppProtocol.V201);
            ChargerProtocolStore.MarkBooted(chargePointId);
            var response = new object[]
            {
                3,
                messageId,
                new
                {
                    status = "Accepted",
                    currentTime = DateTime.UtcNow,
                    interval = 5
                }
            };



            var json = JsonSerializer.Serialize(response);
            var bytes = Encoding.UTF8.GetBytes(json);

            await socket.SendAsync(
                bytes,
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );

            
        }
    }

}
