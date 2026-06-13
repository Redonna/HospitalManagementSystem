using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.API.DTOs
{
    // ── PATIENT ──────────────────────────────────────────────
    public class PatientCreateDto
    {
        [Required, MaxLength(100)] public string FirstName { get; set; } = string.Empty;
        [Required, MaxLength(100)] public string LastName { get; set; } = string.Empty;
        [Required] public DateTime DateOfBirth { get; set; }
        [Required, MaxLength(10)] public string Gender { get; set; } = string.Empty;
        [Required, Phone] public string PhoneNumber { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [MaxLength(300)] public string Address { get; set; } = string.Empty;
    }

    public class PatientUpdateDto
    {
        [MaxLength(100)] public string? FirstName { get; set; }
        [MaxLength(100)] public string? LastName { get; set; }
        [Phone] public string? PhoneNumber { get; set; }
        [EmailAddress] public string? Email { get; set; }
        [MaxLength(300)] public string? Address { get; set; }
    }

    public class PatientReadDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; }
        public int TotalAppointments { get; set; }
    }

    // ── DOCTOR ───────────────────────────────────────────────
    public class DoctorCreateDto
    {
        [Required, MaxLength(100)] public string FirstName { get; set; } = string.Empty;
        [Required, MaxLength(100)] public string LastName { get; set; } = string.Empty;
        [Required, MaxLength(100)] public string Specialization { get; set; } = string.Empty;
        [Required, Phone] public string PhoneNumber { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [MaxLength(300)] public string Department { get; set; } = string.Empty;
    }

    public class DoctorUpdateDto
    {
        [MaxLength(100)] public string? Specialization { get; set; }
        [Phone] public string? PhoneNumber { get; set; }
        [EmailAddress] public string? Email { get; set; }
        [MaxLength(300)] public string? Department { get; set; }
    }

    public class DoctorReadDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public int TotalAppointments { get; set; }
    }

    // ── APPOINTMENT ───────────────────────────────────────────
    public class AppointmentCreateDto
    {
        [Required] public int PatientId { get; set; }
        [Required] public int DoctorId { get; set; }
        [Required] public DateTime AppointmentDate { get; set; }
        [MaxLength(500)] public string Reason { get; set; } = string.Empty;
    }

    public class AppointmentUpdateDto
    {
        public DateTime? AppointmentDate { get; set; }
        [MaxLength(500)] public string? Reason { get; set; }
        [MaxLength(1000)] public string? Notes { get; set; }
        public string? Status { get; set; } // Scheduled, Completed, Cancelled
    }

    public class AppointmentReadDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string DoctorSpecialization { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    // ── AUTH ──────────────────────────────────────────────────
    public class RegisterDto
    {
        [Required, MaxLength(100)] public string Username { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
        [Required, MaxLength(50)] public string Role { get; set; } = string.Empty;
        [EmailAddress] public string Email { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        [Required] public string Username { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
