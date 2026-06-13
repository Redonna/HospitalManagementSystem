using HospitalManagementSystem.API.Data;
using HospitalManagementSystem.API.Models;
using HospitalManagementSystem.API.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HospitalManagementSystem.Tests.Repositories
{
    public class AppointmentRepositoryTests
    {
        private HospitalDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<HospitalDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new HospitalDbContext(options);
        }

        private Patient MakePatient() => new()
        {
            FirstName = "Elizabeth", LastName = "Jones",
            DateOfBirth = new DateTime(1990, 1, 1), Gender = "Female",
            PhoneNumber = "072345678", Email = "elizabeth@test.com", IsActive = true
        };

        private Doctor MakeDoctor() => new()
        {
            FirstName = "James", LastName = "Smith",
            Specialization = "Cardiology", PhoneNumber = "071234567",
            Email = "james@hospital.com", Department = "Cardiology", IsActive = true
        };

        private Appointment MakeAppointment(int patientId, int doctorId) => new()
        {
            PatientId = patientId,
            DoctorId = doctorId,
            AppointmentDate = DateTime.UtcNow.AddDays(7),
            Reason = "Routine checkup",
            Status = AppointmentStatus.Scheduled,
            Notes = "",
            CreatedAt = DateTime.UtcNow
        };

        // ── Happy Path ────────────────────────────────────────────────────────

        [Fact]
        public async Task CreateAsync_PersistsAppointment()
        {
            // Arrange
            using var ctx = CreateContext(nameof(CreateAsync_PersistsAppointment));
            var patient = MakePatient();
            var doctor = MakeDoctor();
            ctx.Patients.Add(patient);
            ctx.Doctors.Add(doctor);
            await ctx.SaveChangesAsync();
            var repo = new AppointmentRepository(ctx);

            // Act
            var result = await repo.CreateAsync(MakeAppointment(patient.Id, doctor.Id));

            // Assert
            Assert.True(result.Id > 0);
            Assert.Equal(1, ctx.Appointments.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsAppointment_WhenFound()
        {
            // Arrange
            using var ctx = CreateContext(nameof(GetByIdAsync_ReturnsAppointment_WhenFound));
            var patient = MakePatient();
            var doctor = MakeDoctor();
            ctx.Patients.Add(patient);
            ctx.Doctors.Add(doctor);
            await ctx.SaveChangesAsync();
            var appointment = MakeAppointment(patient.Id, doctor.Id);
            ctx.Appointments.Add(appointment);
            await ctx.SaveChangesAsync();
            var repo = new AppointmentRepository(ctx);

            // Act
            var result = await repo.GetByIdAsync(appointment.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(appointment.Id, result.Id);
        }

        [Fact]
        public async Task GetByPatientIdAsync_ReturnsPatientAppointments()
        {
            // Arrange
            using var ctx = CreateContext(nameof(GetByPatientIdAsync_ReturnsPatientAppointments));
            var patient = MakePatient();
            var doctor = MakeDoctor();
            ctx.Patients.Add(patient);
            ctx.Doctors.Add(doctor);
            await ctx.SaveChangesAsync();
            ctx.Appointments.Add(MakeAppointment(patient.Id, doctor.Id));
            ctx.Appointments.Add(MakeAppointment(patient.Id, doctor.Id));
            await ctx.SaveChangesAsync();
            var repo = new AppointmentRepository(ctx);

            // Act
            var result = await repo.GetByPatientIdAsync(patient.Id);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllAppointments()
        {
            // Arrange
            using var ctx = CreateContext(nameof(GetAllAsync_ReturnsAllAppointments));
            var patient = MakePatient();
            var doctor = MakeDoctor();
            ctx.Patients.Add(patient);
            ctx.Doctors.Add(doctor);
            await ctx.SaveChangesAsync();
            ctx.Appointments.Add(MakeAppointment(patient.Id, doctor.Id));
            ctx.Appointments.Add(MakeAppointment(patient.Id, doctor.Id));
            await ctx.SaveChangesAsync();
            var repo = new AppointmentRepository(ctx);

            // Act
            var result = await repo.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        // ── Sad Path ──────────────────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            using var ctx = CreateContext(nameof(GetByIdAsync_ReturnsNull_WhenNotFound));
            var repo = new AppointmentRepository(ctx);

            // Act
            var result = await repo.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByPatientIdAsync_ReturnsEmpty_WhenNoAppointments()
        {
            // Arrange
            using var ctx = CreateContext(nameof(GetByPatientIdAsync_ReturnsEmpty_WhenNoAppointments));
            var repo = new AppointmentRepository(ctx);

            // Act
            var result = await repo.GetByPatientIdAsync(999);

            // Assert
            Assert.Empty(result);
        }
    }
}
