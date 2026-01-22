using DotNetEnv;
using esyasoft.mobility.CHRGUP.service.ocpp.CanonicalEvents;
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


            var evseId = payload.GetProperty("evseId").GetInt32();


            double energy = 0;
            double soc = 0;
            var sessionId = payload
            .GetProperty("transactionId")
            .GetInt32()
            .ToString();

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
                        energy = value;
                    }
                    if (measurand == "SoC")
                    {
                        soc = value;
                    }

                    var meterEvent = new MeterValueEvent
                    {
                        ChargerId = chargePointId,
                        SessionId = sessionId,
                        Timestamp = timestamp,
                        EnergyKwh = energy,
                        SOC = soc
                    };


                    await RabbitMqEventPublisher.PublishAsync(
                        "event.meter.value",
                        meterEvent
                    );
                }
            }
        }
    }
}