namespace esyasoft.mobility.CHRGUP.service.api.DTOs.Messaging
{
    public class RemoteStopCommand
    {
        public string ChargerId { get; set; }
        public string SessionId { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}
