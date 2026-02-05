using esyasoft.mobility.CHRGUP.service.api.DTOs.Common;
using esyasoft.mobility.CHRGUP.service.api.DTOs.Log;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace esyasoft.mobility.CHRGUP.service.api.Services
{
    public class LogService : ILogService
    {
        private readonly AppDbContext _db;

        public LogService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<PaginatedResponseDto<LogResponseDto>> GetPagedAsync(
            int page,
            int pageSize,
            string? sessionId,
            string? chargerId,
            string? driverId)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var query = _db.logs.AsQueryable();

            if (!string.IsNullOrEmpty(sessionId))
                query = query.Where(l => l.SessionId == sessionId);

            if (!string.IsNullOrEmpty(chargerId))
                query = query.Where(l => l.ChargerId == chargerId);

            if (!string.IsNullOrEmpty(driverId))
                query = query.Where(l => l.DriverId == driverId);

            var totalRecords = await query.CountAsync();

            var logs = await query
                .OrderByDescending(l => l.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new LogResponseDto
                {
                    Id = l.Id,
                    Timestamp = l.Timestamp,
                    Source = l.Source,
                    EventType = l.EventType,
                    Message = l.Message,
                    ChargerId = l.ChargerId,
                    SessionId = l.SessionId,
                    DriverId = l.DriverId
                })
                .ToListAsync();

            return new PaginatedResponseDto<LogResponseDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                Data = logs
            };
        }

        public async Task<LogResponseDto> GetByIdAsync(Guid id)
        {
            return await _db.logs
                .Where(l => l.Id == id)
                .Select(l => new LogResponseDto
                {
                    Id = l.Id,
                    Timestamp = l.Timestamp,
                    Source = l.Source,
                    EventType = l.EventType,
                    Message = l.Message,
                    ChargerId = l.ChargerId,
                    SessionId = l.SessionId,
                    DriverId = l.DriverId
                })
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException("Log not found");
        }
    }
}
