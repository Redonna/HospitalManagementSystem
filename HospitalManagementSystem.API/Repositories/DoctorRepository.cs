using HospitalManagementSystem.API.Data;
using HospitalManagementSystem.API.Models;
using HospitalManagementSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.API.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly HospitalDbContext _context;

        public DoctorRepository(HospitalDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await _context.Doctors
                .AsNoTracking()
                .Include(d => d.Appointments)
                .Where(d => d.IsActive)
                .OrderBy(d => d.Specialization)
                .ToListAsync();
        }

        public async Task<Doctor?> GetByIdAsync(int id)
        {
            return await _context.Doctors
                .AsNoTracking()
                .Include(d => d.Appointments)
                .FirstOrDefaultAsync(d => d.Id == id && d.IsActive);
        }

        public async Task<Doctor> CreateAsync(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
            return doctor;
        }

        public async Task<Doctor?> UpdateAsync(int id, Doctor updated)
        {
            var existing = await _context.Doctors.FindAsync(id);
            if (existing == null || !existing.IsActive) return null;

            existing.Specialization = updated.Specialization;
            existing.PhoneNumber = updated.PhoneNumber;
            existing.Email = updated.Email;
            existing.Department = updated.Department;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return false;

            doctor.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Doctors.AnyAsync(d => d.Id == id && d.IsActive);
        }
    }
}
