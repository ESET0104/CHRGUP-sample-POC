using esyasoft.mobility.CHRGUP.service.api.DTOs.Charger;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.core.Metadata;
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
            return Ok(await _chargerService.GetAllAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromQuery] string locationId)
        {
            return Ok(await _chargerService.RegisterAsync(locationId));
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
            await _chargerService.UpdateHeartbeatAsync(chargerId, DateTime.Now);
            return NoContent();
        }

        [HttpPost("{chargerId}/remote-start")]
        public async Task<IActionResult> RemoteStart(
            string chargerId,
            [FromBody] StartChargingRequestDto dto)
        {
            await _chargerService.RemoteStartAsync(chargerId, dto);
            return Accepted(new
            {
                message = "Remote start request accepted",
                chargerId
            });
        }

        [HttpPost("{chargerId}/remote-stop")]
        public async Task<IActionResult> RemoteStop(
            string chargerId,
            [FromBody] StopChargingRequestDto dto)
        {
            await _chargerService.RemoteStopAsync(chargerId, dto);
            return Accepted(new
            {
                message = "Remote stop request accepted",
                chargerId
            });
        }
    }
}
