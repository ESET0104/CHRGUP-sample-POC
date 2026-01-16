using esyasoft.mobility.CHRGUP.service.api.DTOs.Charger;
using esyasoft.mobility.CHRGUP.service.api.DTOs.ChargingSession;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.api.Services;
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
        private readonly IChargingSessionService _chargingSessionService;

        public ChargerController(
            IChargerService chargerService,
            IChargingSessionService chargingSessionService)
        {
            _chargerService = chargerService;
            _chargingSessionService = chargingSessionService;
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
            await _chargerService.UpdateHeartbeatAsync(
                chargerId,
                DateTime.UtcNow);

            return NoContent();
        }


        [HttpPost("{chargerId}/remote-start")]
        public async Task<IActionResult> RemoteStart(
            string chargerId,
            [FromBody] RemoteStartRequestDto dto)
        {
            var session = await _chargingSessionService.StartAsync(
                new StartChargingRequestDto
                {
                    ChargerId = chargerId,
                    DriverId = dto.DriverId
                });

            return Accepted(new
            {
                message = "Remote start request accepted",
                chargerId,
                sessionId = session.SessionId
            });
        }

        [HttpPost("{chargerId}/remote-stop")]
        public async Task<IActionResult> RemoteStop(
            string chargerId,
            [FromBody] RemoteStopRequestDto dto)
        {
            await _chargingSessionService.StopAsync(dto.SessionId);

            return Accepted(new
            {
                message = "Remote stop request accepted",
                chargerId,
                sessionId = dto.SessionId
            });
        }
    }
}
