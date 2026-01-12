using System.ComponentModel.DataAnnotations.Schema;

namespace esyasoft.mobility.CHRGUP.service.core.Models
{
    [Table("t_log", Schema = "transactions")]
    public class Log
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Source { get; set; } = null!;
        public string EventType { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string? ChargerId { get; set; }
        public string? SessionId { get; set; }
        public string? DriverId { get; set; }

    }
}
