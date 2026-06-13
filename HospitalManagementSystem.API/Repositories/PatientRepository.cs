using HospitalManagementSystem.API.Data;
using HospitalManagementSystem.API.Models;
using HospitalManagementSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.API.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly HospitalDbContext _context;

        public PatientRepository(HospitalDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _context.Patients
                .AsNoTracking()
                .Include(p => p.Appointments)
                .Where(p => p.IsActive)
                .OrderBy(p => p.LastName)
                .ToListAsync();
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            return await _context.Patients
                .AsNoTracking()
                .Include(p => p.Appointments)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        }

        public async Task<Patient> CreateAsync(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task<Patient?> UpdateAsync(int id, Patient updated)
        {
            var existing = await _context.Patients.FindAsync(id);
            if (existing == null || !existing.IsActive) return null;

            existing.FirstName = updated.FirstName;
            existing.LastName = updated.LastName;
            existing.PhoneNumber = updated.PhoneNumber;
            existing.Email = updated.Email;
            existing.Address = updated.Address;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return false;

            // Soft delete
            patient.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Patients.AnyAsync(p => p.Id == id && p.IsActive);
        }
    }
}
