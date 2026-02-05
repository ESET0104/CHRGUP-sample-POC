using esyasoft.mobility.CHRGUP.service.api.DTOs.Users;

namespace esyasoft.mobility.CHRGUP.service.api.Interfaces
{
    public interface ISupervisorService
    {
        Task<UserResponseDto> CreateAsync(CreateUserDto dto);

        Task<List<UserResponseDto>> GetAllAsync();

        Task<UserResponseDto> GetByIdAsync(string id);

        Task<UserResponseDto> UpdateAsync(string id, UpdateUserDto dto);

        Task DeleteAsync(string id);

    }
}
