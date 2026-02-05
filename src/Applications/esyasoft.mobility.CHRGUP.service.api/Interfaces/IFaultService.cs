using esyasoft.mobility.CHRGUP.service.api.DTOs.Fault;

namespace esyasoft.mobility.CHRGUP.service.api.Interfaces
{
    public interface IFaultService
    {
        Task<List<FaultResponseDto>> GetAllAsync();
        Task<List<FaultResponseDto>> GetByChargerIdAsync(string chargerId);
        Task<FaultResponseDto> GetByIdAsync(string id);
    }
}
