using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace esyasoft.mobility.CHRGUP.service.api.Controllers
{
    [ApiController]
    [Route("api/logs")]
    public class LogController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogController(ILogService logService)
        {
            _logService = logService;
        }

        // GET /api/logs?page=1&pageSize=20&chargerId=CHG001
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? sessionId = null,
            [FromQuery] string? chargerId = null,
            [FromQuery] string? driverId = null)
        {
            return Ok(await _logService.GetPagedAsync(
                page,
                pageSize,
                sessionId,
                chargerId,
                driverId));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _logService.GetByIdAsync(id));
        }
    }
}
