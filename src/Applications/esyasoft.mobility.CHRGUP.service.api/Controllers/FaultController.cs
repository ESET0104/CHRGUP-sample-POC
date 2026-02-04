using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace esyasoft.mobility.CHRGUP.service.api.Controllers
{
    [ApiController]
    [Route("api/faults")]
    public class FaultController : ControllerBase
    {
        private readonly IFaultService _faultService;

        public FaultController(IFaultService faultService)
        {
            _faultService = faultService;
        }

        // GET /api/faults
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? chargerId)
        {
            if (!string.IsNullOrEmpty(chargerId))
                return Ok(await _faultService.GetByChargerIdAsync(chargerId));

            return Ok(await _faultService.GetAllAsync());
        }

        // GET /api/faults/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            return Ok(await _faultService.GetByIdAsync(id));
        }
    }
}
