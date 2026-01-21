namespace esyasoft.mobility.CHRGUP.service.api.DTOs.Messaging
{
    public class RemoteStartCommand
    {
        public string ChargerId { get; set; }
        public int ConnectorId { get; set; }
        public string DriverId { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}
