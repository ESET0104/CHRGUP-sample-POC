using System.ComponentModel.DataAnnotations;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Data
{
    public class OcppCharger
    {
        [Required]
        [Key]
        public string ChargerId { get; set; }
        public string TenantId { get; set; }
        public bool IsEnabled { get; set; } = true;
        public DateTime? LastSeen { get; set; }
    }
}
