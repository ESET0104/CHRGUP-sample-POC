using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
using System.Text.Json;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp16.Handlers
{
    public class MeterHandler
    {
        public static async Task Handle(JsonElement payload, string chargerId)
        {
            

            var session = CanonicalSessionStore.GetOrCreate(chargerId, OcppProtocol.V16,1);
            if (!session.Active || session.SessionId == null)
                return;


            foreach (var sample in payload.EnumerateArray())
            {
                var value = double.Parse(sample.GetProperty("value").GetString());
                //var measurand = sample.GetProperty("measurand").GetString();
                var measurand = sample.TryGetProperty("measurand", out var m)
                    ? m.GetString()
                    : null;

                if (measurand == "Energy.Active.Import.Register" || measurand == null)
                {
                    session.EnergyKwh = value;
                }
                if (measurand == "SoC")
                {
                    session.Soc = value;
                }
            }


            await RabbitMqEventPublisher.PublishAsync(
                "event.meter.value",
                new { ChargerId = chargerId, SessionId = session.SessionId, EvseId = 1, EnergyKwh = session.EnergyKwh, Soc = session.Soc });
        }
    }
}
