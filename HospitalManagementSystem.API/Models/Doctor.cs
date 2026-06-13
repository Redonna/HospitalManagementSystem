using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.API.Models
{
    public class Doctor
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Specialization { get; set; } = string.Empty;

        [Required, Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MaxLength(300)]
        public string Department { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
