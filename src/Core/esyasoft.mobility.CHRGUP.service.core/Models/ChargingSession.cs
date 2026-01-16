using esyasoft.mobility.CHRGUP.service.core.Metadata;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace esyasoft.mobility.CHRGUP.service.core.Models
{
    [Table("t_chargingSession", Schema = "transactions")]
    public class ChargingSession
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [ForeignKey(nameof(Charger))]
        public string ChargerId { get; set; }
        public Charger Charger { get; set; }

        [Required]
        [ForeignKey(nameof(Driver))]
        public string DriverId { get; set; }
        public Driver Driver { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int InitialCharge { get; set; }

        public int SOC { get; set; }

        public decimal? EnergyConsumedKwh { get; set; }

        [Required]
        public SessionStatus Status { get; set; }

        public DateTime LastMeterUpdate { get; set; }
    }
}
