using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace esyasoft.mobility.CHRGUP.service.core.Models
{
    [Table("c_chargerconfig", Schema = "config")]
    public class ChargerConfig
    {
        [Key] public string ChargerId { get; set; }
        [Required] public string Manufacturer {  get; set; }
        [Required] public string FirmwareVersion { get; set; }
        public double InputPower { get; set; }
        public double OutputPower { get; set; }
        public string ConnectorType { get; set; }
        public int NoOfPorts { get; set; }
    }
}
