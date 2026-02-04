using esyasoft.mobility.CHRGUP.service.api.DTOs.Common;
using esyasoft.mobility.CHRGUP.service.api.DTOs.Log;

namespace esyasoft.mobility.CHRGUP.service.api.Interfaces
{
        public interface ILogService
        {
            Task<PaginatedResponseDto<LogResponseDto>> GetPagedAsync(
                int page,
                int pageSize,
                string? sessionId,
                string? chargerId,
                string? driverId);

            Task<LogResponseDto> GetByIdAsync(Guid id);
        }
    }
