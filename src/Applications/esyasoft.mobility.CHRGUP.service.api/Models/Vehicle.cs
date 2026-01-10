using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace esyasoft.mobility.CHRGUP.service.api.Models
{
    [Index(nameof(RegistrationNumber), IsUnique = true)]
    [Index(nameof(VIN), IsUnique = true)]
    [Table("m_vehicle", Schema = "master")]
    public class Vehicle
    {
        [Key] public string Id { get; set; }
        [Required] public string VehicleName { get; set; }
        [Required] public string VIN { get; set; }
        public string MakeandModel { get; set; }
        [Required] public string RegistrationNumber { get; set; }
        public int? RangeKm { get; set; }
        public double BatteryCapacityKwh { get; set; }
        public double MaxChargeRateKw { get; set; }

    }
}
