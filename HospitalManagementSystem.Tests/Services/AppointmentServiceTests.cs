using AutoMapper;
using HospitalManagementSystem.API.DTOs;
using HospitalManagementSystem.API.Mappings;
using HospitalManagementSystem.API.Models;
using HospitalManagementSystem.API.Repositories.Interfaces;
using HospitalManagementSystem.API.Services;
using NSubstitute;
using Xunit;

namespace HospitalManagementSystem.Tests.Services
{
    public class AppointmentServiceTests
    {
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IPatientRepository _patientRepo;
        private readonly IDoctorRepository _doctorRepo;
        private readonly IMapper _mapper;
        private readonly AppointmentService _service;

        public AppointmentServiceTests()
        {
            _appointmentRepo = Substitute.For<IAppointmentRepository>();
            _patientRepo = Substitute.For<IPatientRepository>();
            _doctorRepo = Substitute.For<IDoctorRepository>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
            _service = new AppointmentService(_appointmentRepo, _patientRepo, _doctorRepo, _mapper);
        }

        private Appointment MakeAppointment(int id = 1) => new()
        {
            Id = id,
            PatientId = 1,
            DoctorId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(7),
            Reason = "Routine checkup",
            Status = AppointmentStatus.Scheduled,
            Notes = "",
            CreatedAt = DateTime.UtcNow,
            Patient = new Patient
            {
                Id = 1, FirstName = "Elizabeth", LastName = "Jones",
                DateOfBirth = new DateTime(1990, 1, 1), Gender = "Female",
                PhoneNumber = "072345678", Email = "elizabeth@test.com",
                IsActive = true, Appointments = new List<Appointment>()
            },
            Doctor = new Doctor
            {
                Id = 1, FirstName = "James", LastName = "Smith",
                Specialization = "Cardiology", PhoneNumber = "071234567",
                Email = "james@hospital.com", Department = "Cardiology",
                IsActive = true, Appointments = new List<Appointment>()
            }
        };

        // ── Happy Path ────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAllAsync_ReturnsMappedDtos()
        {
            // Arrange
            _appointmentRepo.GetAllAsync().Returns(new List<Appointment> { MakeAppointment(1), MakeAppointment(2) });

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            await _appointmentRepo.Received(1).GetAllAsync();
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsDto_WhenFound()
        {
            // Arrange
            _appointmentRepo.GetByIdAsync(1).Returns(MakeAppointment(1));

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Elizabeth Jones", result.PatientName);
        }

        [Fact]
        public async Task GetByPatientIdAsync_ReturnsDtos()
        {
            // Arrange
            _appointmentRepo.GetByPatientIdAsync(1).Returns(new List<Appointment> { MakeAppointment(1) });

            // Act
            var result = await _service.GetByPatientIdAsync(1);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task CreateAsync_ReturnsError_WhenDateIsInPast()
        {
            // Arrange
            var dto = new AppointmentCreateDto
            {
                PatientId = 1,
                DoctorId = 1,
                AppointmentDate = DateTime.UtcNow.AddDays(-1),
                Reason = "Checkup"
            };

            // Act
            var (result, error) = await _service.CreateAsync(dto);

            // Assert
            Assert.Null(result);
            Assert.Equal("Appointment date must be in the future.", error);
        }

        [Fact]
        public async Task CreateAsync_ReturnsError_WhenPatientNotFound()
        {
            // Arrange
            var dto = new AppointmentCreateDto
            {
                PatientId = 99,
                DoctorId = 1,
                AppointmentDate = DateTime.UtcNow.AddDays(7),
                Reason = "Checkup"
            };
            _patientRepo.ExistsAsync(99).Returns(false);

            // Act
            var (result, error) = await _service.CreateAsync(dto);

            // Assert
            Assert.Null(result);
            Assert.Contains("Patient", error);
        }

        [Fact]
        public async Task CreateAsync_ReturnsError_WhenDoctorNotFound()
        {
            // Arrange
            var dto = new AppointmentCreateDto
            {
                PatientId = 1,
                DoctorId = 99,
                AppointmentDate = DateTime.UtcNow.AddDays(7),
                Reason = "Checkup"
            };
            _patientRepo.ExistsAsync(1).Returns(true);
            _doctorRepo.ExistsAsync(99).Returns(false);

            // Act
            var (result, error) = await _service.CreateAsync(dto);

            // Assert
            Assert.Null(result);
            Assert.Contains("Doctor", error);
        }

        [Fact]
        public async Task CancelAsync_ReturnsTrue_WhenAppointmentExists()
        {
            // Arrange
            var appointment = MakeAppointment(1);
            _appointmentRepo.GetByIdAsync(1).Returns(appointment);
            _appointmentRepo.UpdateAsync(1, Arg.Any<Appointment>()).Returns(appointment);

            // Act
            var result = await _service.CancelAsync(1);

            // Assert
            Assert.True(result);
            await _appointmentRepo.Received(1).UpdateAsync(1, Arg.Any<Appointment>());
        }

        // ── Sad Path ──────────────────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            _appointmentRepo.GetByIdAsync(99).Returns((Appointment?)null);

            // Act
            var result = await _service.GetByIdAsync(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CancelAsync_ReturnsFalse_WhenNotFound()
        {
            // Arrange
            _appointmentRepo.GetByIdAsync(99).Returns((Appointment?)null);

            // Act
            var result = await _service.CancelAsync(99);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsError_WhenAppointmentCancelled()
        {
            // Arrange
            var appointment = MakeAppointment(1);
            appointment.Status = AppointmentStatus.Cancelled;
            _appointmentRepo.GetByIdAsync(1).Returns(appointment);

            // Act
            var (result, error) = await _service.UpdateAsync(1, new AppointmentUpdateDto());

            // Assert
            Assert.Null(result);
            Assert.Equal("Cannot update a cancelled appointment.", error);
        }
    }
}
