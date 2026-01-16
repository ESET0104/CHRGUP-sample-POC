using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
using System.Text.Json;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp16.Handlers
{
    public class MeterHandler
    {
        public static async Task Handle(JsonElement payload, string chargerId)
        {
            var value = double.Parse(payload[0].GetProperty("value").GetString());

            var session = CanonicalSessionStore.GetOrCreate(chargerId, OcppProtocol.V16);
            session.EnergyKwh = value;

            await RabbitMqEventPublisher.PublishAsync(
                "event.meter.value",
                new { ChargerId = chargerId, SessionId = session.SessionId, EnergyKwh = value });
        }
    }
}
