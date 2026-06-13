using AutoMapper;
using HospitalManagementSystem.API.DTOs;
using HospitalManagementSystem.API.Models;
using HospitalManagementSystem.API.Repositories.Interfaces;
using HospitalManagementSystem.API.Services.Interfaces;

namespace HospitalManagementSystem.API.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public PatientService(IPatientRepository repository, IUserRepository userRepository, IMapper mapper)
        {
            _repository = repository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PatientReadDto>> GetAllAsync()
        {
            var patients = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<PatientReadDto>>(patients);
        }

        public async Task<PatientReadDto?> GetByIdAsync(int id)
        {
            var patient = await _repository.GetByIdAsync(id);
            return patient == null ? null : _mapper.Map<PatientReadDto>(patient);
        }

        public async Task<PatientReadDto> CreateAsync(PatientCreateDto dto)
        {
            var patient = _mapper.Map<Patient>(dto);
            var created = await _repository.CreateAsync(patient);
            return _mapper.Map<PatientReadDto>(created);
        }

        public async Task<PatientReadDto?> UpdateAsync(int id, PatientUpdateDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            // Apply only provided fields
            if (dto.FirstName != null) existing.FirstName = dto.FirstName;
            if (dto.LastName != null) existing.LastName = dto.LastName;
            if (dto.PhoneNumber != null) existing.PhoneNumber = dto.PhoneNumber;
            if (dto.Email != null) existing.Email = dto.Email;
            if (dto.Address != null) existing.Address = dto.Address;

            var updated = await _repository.UpdateAsync(id, existing);
            return updated == null ? null : _mapper.Map<PatientReadDto>(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var patient = await _repository.GetByIdAsync(id);
            if (patient == null) return false;
            await _userRepository.DeactivateByEmailAsync(patient.Email);
            return await _repository.DeleteAsync(id);
        }
    }
}
