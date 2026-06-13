using HospitalManagementSystem.API.Models;

namespace HospitalManagementSystem.API.Repositories.Interfaces
{
    public interface IDoctorRepository
    {
        Task<IEnumerable<Doctor>> GetAllAsync();
        Task<Doctor?> GetByIdAsync(int id);
        Task<Doctor> CreateAsync(Doctor doctor);
        Task<Doctor?> UpdateAsync(int id, Doctor doctor);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
