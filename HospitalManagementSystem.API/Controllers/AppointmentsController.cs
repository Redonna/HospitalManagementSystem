using HospitalManagementSystem.API.DTOs;
using HospitalManagementSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _service;

        public AppointmentsController(IAppointmentService service)
        {
            _service = service;
        }

        /// <summary>Get all appointments</summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetAll()
        {
            var appointments = await _service.GetAllAsync();
            return Ok(appointments);
        }

        /// <summary>Get appointment by ID</summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Doctor,Patient")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0) return BadRequest("ID must be greater than 0.");
            var appointment = await _service.GetByIdAsync(id);
            return appointment == null ? NotFound($"Appointment with ID {id} not found.") : Ok(appointment);
        }

        /// <summary>Get all appointments for a specific patient</summary>
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Admin,Doctor,Patient")]
        public async Task<IActionResult> GetByPatient(int patientId)
        {
            if (patientId <= 0) return BadRequest("Patient ID must be greater than 0.");
            var appointments = await _service.GetByPatientIdAsync(patientId);
            return Ok(appointments);
        }

        /// <summary>Get all appointments for a specific doctor</summary>
        [HttpGet("doctor/{doctorId}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetByDoctor(int doctorId)
        {
            if (doctorId <= 0) return BadRequest("Doctor ID must be greater than 0.");
            var appointments = await _service.GetByDoctorIdAsync(doctorId);
            return Ok(appointments);
        }

        /// <summary>Schedule a new appointment</summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Doctor,Patient")]
        public async Task<IActionResult> Create([FromBody] AppointmentCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (result, error) = await _service.CreateAsync(dto);
            if (error != null) return BadRequest(error);

            return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
        }

        /// <summary>Update an appointment (reschedule, add notes, change status)</summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Update(int id, [FromBody] AppointmentUpdateDto dto)
        {
            if (id <= 0) return BadRequest("ID must be greater than 0.");
            var (result, error) = await _service.UpdateAsync(id, dto);
            if (error != null) return BadRequest(error);
            return result == null ? NotFound($"Appointment with ID {id} not found.") : Ok(result);
        }

        /// <summary>Cancel an appointment</summary>
        [HttpPatch("{id}/cancel")]
        [Authorize(Roles = "Admin,Doctor,Patient")]
        public async Task<IActionResult> Cancel(int id)
        {
            if (id <= 0) return BadRequest("ID must be greater than 0.");
            var cancelled = await _service.CancelAsync(id);
            return cancelled ? NoContent() : NotFound($"Appointment with ID {id} not found.");
        }
    }
}
