using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using esyasoft.mobility.CHRGUP.service.core.Metadata;


namespace esyasoft.mobility.CHRGUP.service.core.Models
{
    [Table("s_driver", Schema = "sso")]
    public class Driver
    {
        [Key] public string Id { get; set; }
        [Required] public string FullName { get; set; }
        [Required] public string Email { get; set; }
        [Required] public string Password { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [Required] public DriverStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastActiveAt { get; set; }
        [ForeignKey("Vehicle")] public string? VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }
    }
}
