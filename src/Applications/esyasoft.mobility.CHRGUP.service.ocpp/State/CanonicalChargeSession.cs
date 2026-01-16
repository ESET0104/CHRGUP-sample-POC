namespace esyasoft.mobility.CHRGUP.service.ocpp.State
{
    public class CanonicalChargeSession
    {
        public string ChargerId { get; set; }
        public string SessionId { get; set; }
        public int EvseID { get; set; } = 1;
        public string UserId { get; set; }   // VIN or RFID
        public OcppProtocol Protocol { get; set; }
        public double EnergyKwh { get; set; }
        public double Soc { get; set; }
        public bool Active { get; set; }
    }
}
