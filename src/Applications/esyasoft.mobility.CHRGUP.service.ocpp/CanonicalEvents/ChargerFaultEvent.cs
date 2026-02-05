using esyasoft.mobility.CHRGUP.service.core.Metadata;

namespace esyasoft.mobility.CHRGUP.service.ocpp.CanonicalEvents
{
    public class ChargerFaultEvent
    {
        public string ChargerId { get; init; }
        public string FaultCode { get; init; }
        public DateTime Timestamp { get; init; }
        public FaultSeverity Severity { get; internal set; }
    }
}
