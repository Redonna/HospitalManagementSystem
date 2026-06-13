using AutoMapper;
using HospitalManagementSystem.API.DTOs;
using HospitalManagementSystem.API.Models;
using HospitalManagementSystem.API.Repositories.Interfaces;
using HospitalManagementSystem.API.Services.Interfaces;

namespace HospitalManagementSystem.API.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public DoctorService(IDoctorRepository repository, IUserRepository userRepository, IMapper mapper)
        {
            _repository = repository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DoctorReadDto>> GetAllAsync()
        {
            var doctors = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<DoctorReadDto>>(doctors);
        }

        public async Task<DoctorReadDto?> GetByIdAsync(int id)
        {
            var doctor = await _repository.GetByIdAsync(id);
            return doctor == null ? null : _mapper.Map<DoctorReadDto>(doctor);
        }

        public async Task<DoctorReadDto> CreateAsync(DoctorCreateDto dto)
        {
            var doctor = _mapper.Map<Doctor>(dto);
            var created = await _repository.CreateAsync(doctor);
            return _mapper.Map<DoctorReadDto>(created);
        }

        public async Task<DoctorReadDto?> UpdateAsync(int id, DoctorUpdateDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            if (dto.Specialization != null) existing.Specialization = dto.Specialization;
            if (dto.PhoneNumber != null) existing.PhoneNumber = dto.PhoneNumber;
            if (dto.Email != null) existing.Email = dto.Email;
            if (dto.Department != null) existing.Department = dto.Department;

            var updated = await _repository.UpdateAsync(id, existing);
            return updated == null ? null : _mapper.Map<DoctorReadDto>(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var doctor = await _repository.GetByIdAsync(id);
            if (doctor == null) return false;
            await _userRepository.DeactivateByEmailAsync(doctor.Email);
            return await _repository.DeleteAsync(id);
        }
    }
}
