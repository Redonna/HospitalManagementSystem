using HospitalManagementSystem.API.DTOs;
using HospitalManagementSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _service;

        public PatientsController(IPatientService service)
        {
            _service = service;
        }

        /// <summary>Get all active patients</summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetAll()
        {
            var patients = await _service.GetAllAsync();
            return Ok(patients);
        }

        /// <summary>Get a patient by ID</summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Doctor,Patient")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0) return BadRequest("ID must be greater than 0.");
            var patient = await _service.GetByIdAsync(id);
            return patient == null ? NotFound($"Patient with ID {id} not found.") : Ok(patient);
        }

        /// <summary>Register a new patient</summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] PatientCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>Update patient information</summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Update(int id, [FromBody] PatientUpdateDto dto)
        {
            if (id <= 0) return BadRequest("ID must be greater than 0.");
            var updated = await _service.UpdateAsync(id, dto);
            return updated == null ? NotFound($"Patient with ID {id} not found.") : Ok(updated);
        }

        /// <summary>Soft-delete a patient</summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest("ID must be greater than 0.");
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound($"Patient with ID {id} not found.");
        }
    }
}
