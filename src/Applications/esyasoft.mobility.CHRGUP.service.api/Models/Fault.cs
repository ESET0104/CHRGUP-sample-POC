using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace esyasoft.mobility.CHRGUP.service.api.Models
{
    [Table("t_fault", Schema = "transactions")]
    public class Fault
    {
        [Key] public string Id { get; set; }
        [Required][ForeignKey("Charger")] public string ChargerId { get; set; }
        public Charger Charger { get; set; }
        [Required] public string FaultCode { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
