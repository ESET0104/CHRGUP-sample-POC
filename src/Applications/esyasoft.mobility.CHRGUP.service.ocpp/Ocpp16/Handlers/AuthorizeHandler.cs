using System.Text.Json;
using System.Net.WebSockets;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using System.Text;


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
                return;
            }


            var evseId = evseEl.GetInt32();
            //
            AuthRequestStore.Register(messageId, chargerId, evseId, socket);
            //

            //VinAuthorizationStore.Register(messageId, socket);
            //
            await RabbitMqEventPublisher.PublishAsync(
                //"rfid.authorization.request",
                "authorization.request",
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
