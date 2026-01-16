using esyasoft.mobility.CHRGUP.service.api.DTOs.ChargingSession;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace esyasoft.mobility.CHRGUP.service.api.Controllers
{
    [ApiController]
    [Route("api/charging-sessions")]
    public class ChargingSessionController : ControllerBase
    {
        private readonly IChargingSessionService _service;

        public ChargingSessionController(IChargingSessionService service)
        {
            _service = service;
        }

        [HttpPost("start")]
        public async Task<IActionResult> Start(StartChargingRequestDto dto)
        {
            var session = await _service.StartAsync(dto);
            return Accepted(session);
        }

        [HttpPost("stop")]
        public async Task<IActionResult> Stop(StopChargingRequestDto dto)
        {
            await _service.StopAsync(dto.SessionId);
            return Accepted();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(await _service.GetByIdAsync(id));
        }
    }
}
