using HospitalManagementSystem.API.Models;

namespace HospitalManagementSystem.API.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<Appointment?> GetByIdAsync(int id);
        Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId);
        Task<Appointment> CreateAsync(Appointment appointment);
        Task<Appointment?> UpdateAsync(int id, Appointment appointment);
        Task<bool> DeleteAsync(int id);
    }
}
