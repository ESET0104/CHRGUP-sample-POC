using esyasoft.mobility.CHRGUP.service.core.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace esyasoft.mobility.CHRGUP.service.core.Helpers
{
    public static class FaultSeverityClassifier
    {
        public static FaultSeverity Classify(
            string? errorCode,
            string? status,
            string? additionalInfo = null)
        {
            if (errorCode is not null)
            {
                return errorCode switch
                {
                    "GroundFailure" => FaultSeverity.Critical,
                    "HighTemperature" => FaultSeverity.Critical,
                    "OverCurrentFailure" => FaultSeverity.Critical,
                    "EmergencyStop" => FaultSeverity.Critical,
                    "EVCommunicationError" => FaultSeverity.Critical,

                    "ConnectorLockFailure" => FaultSeverity.High,
                    "PowerSwitchFailure" => FaultSeverity.High,
                    "ReaderFailure" => FaultSeverity.High,
                    "UnderVoltage" => FaultSeverity.High,
                    "OverVoltage" => FaultSeverity.High,
                    "InternalError" => FaultSeverity.High,

                    "WeakSignal" => FaultSeverity.Medium,
                    "LocalListConflict" => FaultSeverity.Medium,
                    "OtherError" => FaultSeverity.Medium,

                    "NoError" => FaultSeverity.Low,
                    _ => FaultSeverity.Medium
                };
            }

            if (status == "Faulted")
                return FaultSeverity.High;

            return FaultSeverity.Medium;
        }
    }
}
