using esyasoft.mobility.CHRGUP.service.api.DTOs.Users;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace esyasoft.mobility.CHRGUP.service.api.Controllers
{
    [ApiController]
    [Route("api/supervisors")]
    [Authorize(Roles = "admin,manager")]
    public class SupervisorController : ControllerBase
    {
        private readonly ISupervisorService _service;

        public SupervisorController(ISupervisorService service)

        {

            _service = service;

        }

        [HttpPost]

        public async Task<IActionResult> Create(CreateUserDto dto)

            => Ok(await _service.CreateAsync(dto));

        [HttpGet]

        public async Task<IActionResult> GetAll()

            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]

        public async Task<IActionResult> GetById(string id)

            => Ok(await _service.GetByIdAsync(id));

        [HttpPut("{id}")]

        public async Task<IActionResult> Update(string id, UpdateUserDto dto)

            => Ok(await _service.UpdateAsync(id, dto));

        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(string id)

        {

            await _service.DeleteAsync(id);

            return NoContent();

        }

    }
}
