using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp201
{
    public class OcppMessage
    {
        public int MessageType { get; set; }
        public string MessageId { get; set; }
        public string Action { get; set; }
        public JsonElement Payload { get; set; }

        public static OcppMessage Parse(string json)
        {
            var arr = JsonSerializer.Deserialize<JsonElement[]>(json);

            var messageType = arr[0].GetInt32();

            if(messageType == 2)
            {
                return new OcppMessage
                {
                    MessageType = arr[0].GetInt32(),
                    MessageId = arr[1].GetString(),
                    Action = arr[2].GetString(),
                    Payload = arr[3]
                };
            }

            else
            {
                return new OcppMessage
                {
                    MessageType = arr[0].GetInt32(),
                    MessageId = arr[1].GetString(),
                    Payload = arr[2]
                };
            }
            
        }

        public static string CreateCallResult(string messageId, object payload)
        {
            return JsonSerializer.Serialize(new object[]
            {
                3,
                messageId,
                payload
            });
        }

        public static string CreateCall(string messageId, string action, object payload)
        {
            return JsonSerializer.Serialize(new object[]
            {
                2,
                messageId,
                action,
                payload
            });
        }

        public static string CreateCallError(string messageId, object payload)
        {
            return JsonSerializer.Serialize(new object[]
            {
                4,
                messageId,
                payload
            });
        }

        public static async Task SendCallResult(
            WebSocket socket,
            string messageId,
            object payload)
        {
            var message = new object[]
            {
                3,
                messageId,
                payload
            };

            var bytes = Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(message));

            await socket.SendAsync(
                bytes,
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }

        public static async Task HandleCallResult(PendingOcppRequest pending, JsonElement payload)
        {
            bool accepted = true;

            if (payload.TryGetProperty("status", out var statusProp))
            {
                accepted = statusProp.GetString() == "Accepted";
            }

            var ev = new
            {
                pending.ChargerId,
                pending.SessionId,
                Accepted = accepted,
                Timestamp = DateTime.Now
            };

            await RabbitMqEventPublisher.PublishAsync(
                pending.Type == PendingOcppRequestType.RemoteStart
                    ? "event.remotestart.result"
                    : "event.remotestop.result",
                ev
            );
        }
    }
}
