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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            return Ok(await _service.GetByIdAsync(id));
        }

        [HttpGet]
        public async Task<IActionResult> GetByCharger([FromQuery] string chargerId)
        {
            return Ok(await _service.GetByChargerAsync(chargerId));
        }
    }
}
