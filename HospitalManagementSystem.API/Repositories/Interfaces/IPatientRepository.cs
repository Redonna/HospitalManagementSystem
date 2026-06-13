using HospitalManagementSystem.API.Models;

namespace HospitalManagementSystem.API.Repositories.Interfaces
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient?> GetByIdAsync(int id);
        Task<Patient> CreateAsync(Patient patient);
        Task<Patient?> UpdateAsync(int id, Patient patient);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
