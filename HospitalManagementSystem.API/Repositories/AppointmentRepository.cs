using HospitalManagementSystem.API.Data;
using HospitalManagementSystem.API.Models;
using HospitalManagementSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.API.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly HospitalDbContext _context;

        public AppointmentRepository(HospitalDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId)
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a => a.DoctorId == doctorId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<Appointment> CreateAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        public async Task<Appointment?> UpdateAsync(int id, Appointment updated)
        {
            var existing = await _context.Appointments.FindAsync(id);
            if (existing == null) return null;

            if (updated.AppointmentDate != default)
                existing.AppointmentDate = updated.AppointmentDate;
            if (!string.IsNullOrEmpty(updated.Reason))
                existing.Reason = updated.Reason;
            if (!string.IsNullOrEmpty(updated.Notes))
                existing.Notes = updated.Notes;

            existing.Status = updated.Status;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return false;

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
