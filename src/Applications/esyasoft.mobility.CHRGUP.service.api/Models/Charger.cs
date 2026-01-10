using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using esyasoft.mobility.CHRGUP.service.api.Metadata;

namespace esyasoft.mobility.CHRGUP.service.api.Models
{
    [Table("m_charger", Schema = "master")]
    public class Charger
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public ChargerStatus Status { get; set; }

        [Required]
        public DateTime LastSeen { get; set; }

        [Required]
        [ForeignKey(nameof(Location))]
        public string LocationId { get; set; }

        public Location Location { get; set; }

    }

}
