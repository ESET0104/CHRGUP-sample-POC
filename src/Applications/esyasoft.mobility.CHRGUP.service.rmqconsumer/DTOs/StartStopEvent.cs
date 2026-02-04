using System;
using System.Collections.Generic;
using System.Text;

namespace esyasoft.mobility.CHRGUP.service.rmqconsumer.DTOs
{
    public class StartStopEvent
    {
        public string ChargerId { get; set; }
        public string SessionId { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }

    }
}
