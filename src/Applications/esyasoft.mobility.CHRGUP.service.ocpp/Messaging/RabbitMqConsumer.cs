using esyasoft.mobility.CHRGUP.service.ocpp.Ocpp201;
using esyasoft.mobility.CHRGUP.service.ocpp.Ocpp16;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
using esyasoft.mobility.CHRGUP.service.ocpp.WebSockets;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Messaging
{
    public class RabbitMqConsumer
    {
        private readonly IChannel _channel;

        public RabbitMqConsumer(IChannel channel)
        {
            _channel = channel;
        }

        public void Start()
        {
            const string exchangeName = "charging_commands_ex";
            const string queueName = "charging.commands";

            _channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: ExchangeType.Direct,
                durable: true
            );

            _channel.QueueDeclareAsync(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false
            );

            _channel.QueueBindAsync(
            queue: queueName,
            exchange: exchangeName,
            routingKey: "event.authorization.result"
            );

            _channel.QueueBindAsync(
                queue: queueName,
                exchange: exchangeName,
                routingKey: "command.start"
            );

            _channel.QueueBindAsync(
                queue: queueName,
                exchange: exchangeName,
                routingKey: "command.stop"
            );

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += HandleMessage;

            _channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: true,
                consumer: consumer
            );
        }

        private async Task HandleMessage(object sender, BasicDeliverEventArgs ea)
        {
            string json;
            JsonElement root;
            try
            {
                json = Encoding.UTF8.GetString(ea.Body.ToArray());
                root = JsonDocument.Parse(json).RootElement;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Invalid RabbitMQ message: {ex.Message}");
                return;
            }

            var routingKey = ea.RoutingKey;

            var evseId = root.TryGetProperty("EvseId", out var e) ? e.GetInt32() : 1;

            if (routingKey == "event.authorization.result")
            {
                var messageId = root.GetProperty("MessageId").GetString();
                var accepted = root.GetProperty("Accepted").GetBoolean();

                if (!AuthRequestStore.TryTake(messageId!, out var auth))
                    return;

                var chargerId_ = auth.ChargerId;
                var protocol_ = ChargerProtocolStore.Get(chargerId_);
                var socket_ = ChargerConnectionManager.GetSocket(chargerId_);

                if (socket_ == null || socket_.State != WebSocketState.Open)
                    return;

                if (protocol_ == OcppProtocol.V201)
                    await Ocpp201Message.SendAuthorizeResult(socket_, messageId!, accepted);
                else
                    await Ocpp16Message.SendAuthorizeResult(socket_, messageId!, accepted);

                Console.WriteLine($"AUTH RESULT ({routingKey}): {messageId} is {(accepted ? "ACCEPTED" : "REJECTED")}");
                return;
            }



            var chargerId = root.GetProperty("ChargerId").GetString();
            var sessionId = root.GetProperty("SessionId").GetString();
            var userId = root.TryGetProperty("DriverId", out var u)
                ? u.GetString()
                : null;

            if (!ChargerProtocolStore.HasBooted(chargerId))
            {
                Console.WriteLine($"Command sent to non-booted charger {chargerId}");
                return;
            }


            if (chargerId == null || sessionId == null)
                return;

            var socket = ChargerConnectionManager.GetSocket(chargerId);
            if (socket == null || socket.State != WebSocketState.Open)
                return;

            var protocol = ChargerProtocolStore.Get(chargerId);


            if (routingKey == "command.start")
            {
                await SendStart(protocol, socket, chargerId, sessionId, userId, evseId);
            }

            else if (routingKey == "command.stop")
            {
                await SendStop(protocol, socket, chargerId, sessionId, evseId);
            }

        }

        private async Task SendStart(OcppProtocol protocol, WebSocket socket, string chargerId, string sessionId, string? userId, int evseId)
        {
            Console.WriteLine($"[START {protocol}] Session={sessionId}");

            var messageId = Guid.NewGuid().ToString();

            RequestStore.Add(messageId, new PendingOcppRequest(
                chargerId,
                sessionId,
                PendingOcppRequestType.RemoteStart
            ));

            if (protocol == OcppProtocol.V201)
            {

                var state = ChargerStateStore.Get(chargerId);
                if (state.IsFaulted)
                {
                    Console.WriteLine("Cannot start: charger is faulted");
                    return;
                }

                await SendOcppCommand(socket, messageId, "RequestStartTransaction", new
                {
                    sessionId,
                    userId
                });
            }
            else
            {
                var state = ChargerStateStore.Get(chargerId);
                if (state.IsFaulted)
                {
                    Console.WriteLine("Cannot start: charger is faulted");
                    return;
                }

                state.ActiveSessionId = sessionId;
                await SendOcppCommand(socket, messageId, "RemoteStartTransaction", new
                {
                    idTag = userId,
                    connectorId = 1
                });
            }
        }

        private async Task SendStop(OcppProtocol protocol, WebSocket socket, string chargerId, string sessionId, int evseId)
        {
            Console.WriteLine($"[STOP {protocol}] Session={sessionId}");
            var messageId = Guid.NewGuid().ToString();

            RequestStore.Add(messageId, new PendingOcppRequest(
                chargerId,
                sessionId,
                PendingOcppRequestType.RemoteStop
            ));

            if (protocol == OcppProtocol.V201)
            {
                await SendOcppCommand(socket, messageId, "RequestStopTransaction", new
                {
                    sessionId
                });
            }
            else
            {
                await SendOcppCommand(socket, messageId, "RemoteStopTransaction", new
                {
                    transactionId = sessionId
                });
            }
        }

        private static async Task SendOcppCommand(
            WebSocket socket,
            string messageId,
            string action,
            object payload)
        {
            var message = new object[]
            {
                2,
                messageId,
                action,
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
    }
}
    


