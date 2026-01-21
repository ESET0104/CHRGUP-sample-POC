using DotNetEnv;
using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
using System.Text.Json;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp201.Handlers
{
    public static class MeterValuesHandler
    {
        public static async Task Handle(
            JsonElement payload,
            string chargePointId)
        {
            //var state = ChargerStateStore.Get(chargePointId);
            //if (state.ActiveSessionId == null)
            //    return;

            var evseId = payload.GetProperty("evseId").GetInt32();

            var session = CanonicalSessionStore.GetOrCreate(chargePointId, OcppProtocol.V201, evseId);
            if (!session.Active || session.SessionId == null)
                return;

            foreach (var meterValue in payload
                .GetProperty("meterValue")
                .EnumerateArray())
            {
                var timestamp = meterValue
                    .GetProperty("timestamp")
                    .GetDateTime();

                foreach (var sampledValue in meterValue
                    .GetProperty("sampledValue")
                    .EnumerateArray())
                {
                    var measurand = sampledValue
                        .GetProperty("measurand")
                        .GetString();

                    if (measurand != "Energy.Active.Import.Register")
                        continue;

                    var value = double.Parse(
                        sampledValue.GetProperty("value").GetString()!
                    );

                    if (measurand == "Energy.Active.Import.Register")
                    {
                        session.EnergyKwh = value;
                    }
                    if (measurand == "SoC")
                    {
                        session.Soc = value;
                    }

                    await RabbitMqEventPublisher.PublishAsync(
                        "event.meter.value",
                        new
                        {
                            //SessionId = state.ActiveSessionId,
                            SessionId = session.SessionId,
                            ChargerId = chargePointId,
                            EvseId = session.EvseId,
                            Timestamp = timestamp,
                            EnergyKwh = session.EnergyKwh,
                            Soc = session.Soc
                        }
                    );
                    session.Active = false;
                }
            }
        }
    }
}