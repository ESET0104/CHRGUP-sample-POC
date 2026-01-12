using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.core.Metadata;
using esyasoft.mobility.CHRGUP.service.core.Models;
using Microsoft.AspNetCore.Mvc;

namespace esyasoft.mobility.CHRGUP.service.api.Controllers
{
    [ApiController]
    [Route("api/chargers")]
    public class ChargerController : ControllerBase
    {
        private readonly IChargerService _chargerService;

        public ChargerController(IChargerService chargerService)
        {
            _chargerService = chargerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var chargers = await _chargerService.GetAllAsync();
            return Ok(chargers);
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromQuery] string locationId)
        {
            var charger = await _chargerService.RegisterAsync(locationId);
            return Ok(charger);
        }


        [HttpPut("{chargerId}/status")]
        public async Task<IActionResult> UpdateStatus(
            string chargerId,
            [FromQuery] ChargerStatus status)
        {
            await _chargerService.UpdateStatusAsync(chargerId, status);
            return NoContent();
        }

        [HttpPut("{chargerId}/heartbeat")]
        public async Task<IActionResult> Heartbeat(string chargerId)
        {
            await _chargerService.UpdateHeartbeatAsync(chargerId, DateTime.UtcNow);
            return NoContent();
        }
    }
}
