namespace esyasoft.mobility.CHRGUP.service.api.DTOs.Fault
{
    public class FaultResponseDto
    {
        public string Id { get; set; }
        public string ChargerId { get; set; }
        public string FaultCode { get; set; }
        public DateTime Timestamp { get; set; }
        public string Severity { get; set; }
    }
}
