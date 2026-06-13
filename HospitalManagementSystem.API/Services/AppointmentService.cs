using AutoMapper;
using HospitalManagementSystem.API.DTOs;
using HospitalManagementSystem.API.Models;
using HospitalManagementSystem.API.Repositories.Interfaces;
using HospitalManagementSystem.API.Services.Interfaces;

namespace HospitalManagementSystem.API.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;

        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IPatientRepository patientRepository,
            IDoctorRepository doctorRepository,
            IMapper mapper)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AppointmentReadDto>> GetAllAsync()
        {
            var appointments = await _appointmentRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<AppointmentReadDto>>(appointments);
        }

        public async Task<AppointmentReadDto?> GetByIdAsync(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            return appointment == null ? null : _mapper.Map<AppointmentReadDto>(appointment);
        }

        public async Task<IEnumerable<AppointmentReadDto>> GetByPatientIdAsync(int patientId)
        {
            var appointments = await _appointmentRepository.GetByPatientIdAsync(patientId);
            return _mapper.Map<IEnumerable<AppointmentReadDto>>(appointments);
        }

        public async Task<IEnumerable<AppointmentReadDto>> GetByDoctorIdAsync(int doctorId)
        {
            var appointments = await _appointmentRepository.GetByDoctorIdAsync(doctorId);
            return _mapper.Map<IEnumerable<AppointmentReadDto>>(appointments);
        }

        public async Task<(AppointmentReadDto? result, string? error)> CreateAsync(AppointmentCreateDto dto)
        {
            // Business rule: appointment must be in the future
            if (dto.AppointmentDate <= DateTime.UtcNow)
                return (null, "Appointment date must be in the future.");

            // Business rule: patient must exist
            if (!await _patientRepository.ExistsAsync(dto.PatientId))
                return (null, $"Patient with ID {dto.PatientId} not found.");

            // Business rule: doctor must exist
            if (!await _doctorRepository.ExistsAsync(dto.DoctorId))
                return (null, $"Doctor with ID {dto.DoctorId} not found.");

            // Business rule: doctor cannot have two appointments at the same time
            var doctorAppointments = await _appointmentRepository.GetByDoctorIdAsync(dto.DoctorId);
            bool conflict = doctorAppointments.Any(a =>
                a.Status != AppointmentStatus.Cancelled &&
                Math.Abs((a.AppointmentDate - dto.AppointmentDate).TotalMinutes) < 30);

            if (conflict)
                return (null, "The doctor already has an appointment within 30 minutes of the requested time.");

            var appointment = _mapper.Map<Appointment>(dto);
            var created = await _appointmentRepository.CreateAsync(appointment);

            // Reload with navigation properties for mapping
            var full = await _appointmentRepository.GetByIdAsync(created.Id);
            return (_mapper.Map<AppointmentReadDto>(full), null);
        }

        public async Task<(AppointmentReadDto? result, string? error)> UpdateAsync(int id, AppointmentUpdateDto dto)
        {
            var existing = await _appointmentRepository.GetByIdAsync(id);
            if (existing == null) return (null, "Appointment not found.");

            if (existing.Status == AppointmentStatus.Cancelled)
                return (null, "Cannot update a cancelled appointment.");

            // Apply updates
            if (dto.AppointmentDate.HasValue)
            {
                if (dto.AppointmentDate.Value <= DateTime.UtcNow)
                    return (null, "Appointment date must be in the future.");
                existing.AppointmentDate = dto.AppointmentDate.Value;
            }
            if (dto.Reason != null) existing.Reason = dto.Reason;
            if (dto.Notes != null) existing.Notes = dto.Notes;
            if (dto.Status != null && Enum.TryParse<AppointmentStatus>(dto.Status, out var status))
                existing.Status = status;

            var updated = await _appointmentRepository.UpdateAsync(id, existing);
            return (updated == null ? null : _mapper.Map<AppointmentReadDto>(updated), null);
        }

        public async Task<bool> CancelAsync(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null) return false;

            appointment.Status = AppointmentStatus.Cancelled;
            await _appointmentRepository.UpdateAsync(id, appointment);
            return true;
        }
    }
}
