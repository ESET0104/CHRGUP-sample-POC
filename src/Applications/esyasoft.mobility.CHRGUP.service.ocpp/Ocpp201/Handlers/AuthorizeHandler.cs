using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.Ocpp16;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp201.Handlers
{
    public static class AuthorizeHandler
    {
        public static async Task Handle(
            string messageId,
        JsonElement payload,
        string chargePointId,
        WebSocket socket)
        {
            var vin = payload
       .GetProperty("idToken")
       .GetProperty("value")
       .GetString();
            if (!payload.TryGetProperty("evseId", out var evseEl) ||
                evseEl.ValueKind != JsonValueKind.Number)
            {
                Console.WriteLine("Invalid Authorize payload: missing or invalid evseId");
                Ocpp201Message.SendAuthorizeResult(socket, messageId, false);
                return;
            }

             
            var evseId = evseEl.GetInt32();

            Console.WriteLine($"VIN authorization request: {vin}");

            AuthRequestStore.Register(messageId, chargePointId,evseId, socket);

            await RabbitMqEventPublisher.PublishAsync(
                "event.authorization.request",
                new
                {
                    ChargerId = chargePointId,
                    Token = vin,
                    TokenType = "VIN",
                    MessageId = messageId
                }
            );
        }
    }
}
