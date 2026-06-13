using HospitalManagementSystem.API.Data;
using HospitalManagementSystem.API.Models;
using HospitalManagementSystem.API.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HospitalManagementSystem.Tests.Repositories
{
    public class DoctorRepositoryTests
    {
        private HospitalDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<HospitalDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new HospitalDbContext(options);
        }

        private Doctor MakeDoctor(string first = "James", string last = "Smith") => new()
        {
            FirstName = first,
            LastName = last,
            Specialization = "Cardiology",
            PhoneNumber = "071234567",
            Email = $"{first.ToLower()}@hospital.com",
            Department = "Cardiology",
            IsActive = true
        };

        // ── Happy Path ────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAllAsync_ReturnsActiveDoctors()
        {
            // Arrange
            using var ctx = CreateContext(nameof(GetAllAsync_ReturnsActiveDoctors));
            ctx.Doctors.Add(MakeDoctor("James", "Smith"));
            var inactive = MakeDoctor("Robert", "Brown");
            inactive.IsActive = false;
            ctx.Doctors.Add(inactive);
            await ctx.SaveChangesAsync();
            var repo = new DoctorRepository(ctx);

            // Act
            var result = await repo.GetAllAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("James", result.First().FirstName);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectDoctor()
        {
            // Arrange
            using var ctx = CreateContext(nameof(GetByIdAsync_ReturnsCorrectDoctor));
            var doctor = MakeDoctor("Sarah", "Connor");
            ctx.Doctors.Add(doctor);
            await ctx.SaveChangesAsync();
            var repo = new DoctorRepository(ctx);

            // Act
            var result = await repo.GetByIdAsync(doctor.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Sarah", result.FirstName);
        }

        [Fact]
        public async Task CreateAsync_PersistsDoctor()
        {
            // Arrange
            using var ctx = CreateContext(nameof(CreateAsync_PersistsDoctor));
            var repo = new DoctorRepository(ctx);
            var doctor = MakeDoctor("William", "Taylor");

            // Act
            var result = await repo.CreateAsync(doctor);

            // Assert
            Assert.True(result.Id > 0);
            Assert.Equal(1, ctx.Doctors.Count());
        }

        [Fact]
        public async Task DeleteAsync_SoftDeletesDoctor()
        {
            // Arrange
            using var ctx = CreateContext(nameof(DeleteAsync_SoftDeletesDoctor));
            var doctor = MakeDoctor();
            ctx.Doctors.Add(doctor);
            await ctx.SaveChangesAsync();
            var repo = new DoctorRepository(ctx);

            // Act
            var result = await repo.DeleteAsync(doctor.Id);

            // Assert
            Assert.True(result);
            var inDb = await ctx.Doctors.FindAsync(doctor.Id);
            Assert.NotNull(inDb);
            Assert.False(inDb!.IsActive);
        }

        // ── Sad Path ──────────────────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            using var ctx = CreateContext(nameof(GetByIdAsync_ReturnsNull_WhenNotFound));
            var repo = new DoctorRepository(ctx);

            // Act
            var result = await repo.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenInactive()
        {
            // Arrange
            using var ctx = CreateContext(nameof(GetByIdAsync_ReturnsNull_WhenInactive));
            var doctor = MakeDoctor();
            doctor.IsActive = false;
            ctx.Doctors.Add(doctor);
            await ctx.SaveChangesAsync();
            var repo = new DoctorRepository(ctx);

            // Act
            var result = await repo.GetByIdAsync(doctor.Id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenDoctorNotFound()
        {
            // Arrange
            using var ctx = CreateContext(nameof(DeleteAsync_ReturnsFalse_WhenDoctorNotFound));
            var repo = new DoctorRepository(ctx);

            // Act
            var result = await repo.DeleteAsync(999);

            // Assert
            Assert.False(result);
        }
    }
}
