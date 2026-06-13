using HospitalManagementSystem.API.Data;
using HospitalManagementSystem.API.Models;
using HospitalManagementSystem.API.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HospitalManagementSystem.Tests.Repositories
{
    public class PatientRepositoryTests
    {
        private HospitalDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<HospitalDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new HospitalDbContext(options);
        }

        private Patient MakePatient(string first = "Ana", string last = "Popova") => new()
        {
            FirstName = first,
            LastName = last,
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = "Female",
            PhoneNumber = "071234567",
            Email = $"{first.ToLower()}@test.com",
            Address = "Skopje",
            IsActive = true
        };

        // ── Happy Path ────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAllAsync_ReturnsActivePatients()
        {
            // Arrange
            using var ctx = CreateContext(nameof(GetAllAsync_ReturnsActivePatients));
            ctx.Patients.Add(MakePatient("Ana", "Popova"));
            var inactive = MakePatient("Ion", "Beka");
            inactive.IsActive = false;
            ctx.Patients.Add(inactive);
            await ctx.SaveChangesAsync();

            var repo = new PatientRepository(ctx);

            // Act
            var result = await repo.GetAllAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("Ana", result.First().FirstName);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectPatient()
        {
            // Arrange
            using var ctx = CreateContext(nameof(GetByIdAsync_ReturnsCorrectPatient));
            var patient = MakePatient("Besa", "Leka");
            ctx.Patients.Add(patient);
            await ctx.SaveChangesAsync();

            var repo = new PatientRepository(ctx);

            // Act
            var result = await repo.GetByIdAsync(patient.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Besa", result.FirstName);
        }

        [Fact]
        public async Task CreateAsync_PersistsPatient()
        {
            // Arrange
            using var ctx = CreateContext(nameof(CreateAsync_PersistsPatient));
            var repo = new PatientRepository(ctx);
            var patient = MakePatient("Drita", "Meka");

            // Act
            var result = await repo.CreateAsync(patient);

            // Assert
            Assert.True(result.Id > 0);
            Assert.Equal(1, ctx.Patients.Count());
        }

        [Fact]
        public async Task DeleteAsync_SoftDeletesPatient()
        {
            // Arrange
            using var ctx = CreateContext(nameof(DeleteAsync_SoftDeletesPatient));
            var patient = MakePatient();
            ctx.Patients.Add(patient);
            await ctx.SaveChangesAsync();
            var repo = new PatientRepository(ctx);

            // Act
            var result = await repo.DeleteAsync(patient.Id);

            // Assert
            Assert.True(result);
            var inDb = await ctx.Patients.FindAsync(patient.Id);
            Assert.NotNull(inDb);
            Assert.False(inDb!.IsActive); // Soft deleted, not removed
        }

        // ── Sad Path ──────────────────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            using var ctx = CreateContext(nameof(GetByIdAsync_ReturnsNull_WhenNotFound));
            var repo = new PatientRepository(ctx);

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
            var patient = MakePatient();
            patient.IsActive = false;
            ctx.Patients.Add(patient);
            await ctx.SaveChangesAsync();
            var repo = new PatientRepository(ctx);

            // Act
            var result = await repo.GetByIdAsync(patient.Id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmpty_WhenDatabaseEmpty()
        {
            // Arrange
            using var ctx = CreateContext(nameof(GetAllAsync_ReturnsEmpty_WhenDatabaseEmpty));
            var repo = new PatientRepository(ctx);

            // Act
            var result = await repo.GetAllAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenPatientNotFound()
        {
            // Arrange
            using var ctx = CreateContext(nameof(DeleteAsync_ReturnsFalse_WhenPatientNotFound));
            var repo = new PatientRepository(ctx);

            // Act
            var result = await repo.DeleteAsync(999);

            // Assert
            Assert.False(result);
        }
    }
}
