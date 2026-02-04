using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.Ocpp201;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;


namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp16.Handlers
{
    public class AuthorizeHandler
    {
        public static async Task Handle(
            JsonElement payload,
            string chargerId,
            string messageId,
            WebSocket socket)
        {
            var rfid = payload.GetProperty("idTag").GetString();
            if (!payload.TryGetProperty("evseId", out var evseEl) ||
                evseEl.ValueKind != JsonValueKind.Number)
            {
                Console.WriteLine("Invalid Authorize payload: missing or invalid evseId");
                Ocpp16Message.SendAuthorizeResult(socket, messageId, false);
                return;
            }
            var evseId = evseEl.GetInt32();

            AuthRequestStore.Register(messageId, chargerId, evseId, socket);

            await RabbitMqEventPublisher.PublishAsync(
                "event.authorization.request",
                new
                {
                    ChargerId = chargerId,
                    Token = rfid,
                    TokenType = "RFID",
                    MessageId = messageId
                });
        }
    }
}
