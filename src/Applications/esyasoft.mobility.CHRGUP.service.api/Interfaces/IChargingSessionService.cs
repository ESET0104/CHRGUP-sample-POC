using esyasoft.mobility.CHRGUP.service.api.Data.DTOs.ChargingSession;

namespace esyasoft.mobility.CHRGUP.service.api.Interfaces
{
    public interface IChargingSessionService
    {
        Task<ChargingSessionResponseDto> StartAsync(StartChargingRequestDto dto);
        Task StopAsync(string sessionId);
        Task<ChargingSessionResponseDto> GetByIdAsync(string sessionId);
    }
}
