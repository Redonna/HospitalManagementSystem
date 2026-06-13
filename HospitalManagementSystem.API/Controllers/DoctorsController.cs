using HospitalManagementSystem.API.DTOs;
using HospitalManagementSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _service;

        public DoctorsController(IDoctorService service)
        {
            _service = service;
        }

        /// <summary>Get all active doctors</summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var doctors = await _service.GetAllAsync();
            return Ok(doctors);
        }

        /// <summary>Get a doctor by ID</summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0) return BadRequest("ID must be greater than 0.");
            var doctor = await _service.GetByIdAsync(id);
            return doctor == null ? NotFound($"Doctor with ID {id} not found.") : Ok(doctor);
        }

        /// <summary>Add a new doctor to the directory</summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] DoctorCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>Update doctor information</summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] DoctorUpdateDto dto)
        {
            if (id <= 0) return BadRequest("ID must be greater than 0.");
            var updated = await _service.UpdateAsync(id, dto);
            return updated == null ? NotFound($"Doctor with ID {id} not found.") : Ok(updated);
        }

        /// <summary>Remove a doctor from the directory</summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest("ID must be greater than 0.");
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound($"Doctor with ID {id} not found.");
        }
    }
}
