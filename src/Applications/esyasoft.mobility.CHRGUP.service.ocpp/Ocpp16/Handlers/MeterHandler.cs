using esyasoft.mobility.CHRGUP.service.core.Helpers;
using esyasoft.mobility.CHRGUP.service.ocpp.CanonicalEvents;
using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.Ocpp201;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text.Json;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp16.Handlers
{
    public class MeterHandler
    {
        public static async Task Handle(JsonElement payload, string chargerId, string messageId, WebSocket socket)
        {
            double energy = 0;
            double soc = 0;

            var sessionId = payload.GetProperty("transactionId").GetString();

            var meterValues = payload.GetProperty("meterValue");

            foreach (var mv in meterValues.EnumerateArray())
            {
                var sampledValues = mv.GetProperty("sampledValue");

                foreach (var sample in sampledValues.EnumerateArray())
                {
                    var value = double.Parse(sample.GetProperty("value").GetString()!);

                    var measurand = sample.TryGetProperty("measurand", out var m)
                        ? m.GetString()
                        : null;

                    if (measurand == "Energy.Active.Import.Register" || measurand == null)
                    {
                        energy = value;
                    }
                    else if (measurand == "SoC")
                    {
                        soc = value;
                    }
                }
            }

            await OcppMessage.SendCallResult(socket, messageId, new{});

            //var response = new object[]
            //{
            //    3,
            //    messageId,
            //    new
            //    {}
            //};

            //await socket.SendAsync(
            //    bytes,
            //    WebSocketMessageType.Text,
            //    true,
            //    CancellationToken.None);

            var ev = new MeterValueEvent
            {
                ChargerId = chargerId,
                SessionId = sessionId,
                Timestamp = DbTime.From(DateTime.Now),
                EnergyKwh = energy,
                SOC = soc
            };

            await RabbitMqEventPublisher.PublishAsync("event.meter.value", ev);


        }
    }
}
