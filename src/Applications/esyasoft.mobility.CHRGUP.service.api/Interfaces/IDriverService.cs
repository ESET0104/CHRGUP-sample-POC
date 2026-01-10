using esyasoft.mobility.CHRGUP.service.api.Data.DTOs.Driver;
using esyasoft.mobility.CHRGUP.service.api.Metadata;
using esyasoft.mobility.CHRGUP.service.api.Models;


namespace esyasoft.mobility.CHRGUP.service.api.Interfaces
{
    public interface IDriverService
    {
        Task<DriverResponseDto> CreateAsync(CreateDriverDto dto);
        Task<List<DriverResponseDto>> GetAllAsync();
        Task<DriverResponseDto> GetByIdAsync(string id);
        Task UpdateStatusAsync(string id, DriverStatus status);
        Task AssignVehicleAsync(string driverId, string vehicleId);
    }
}
