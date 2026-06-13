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
    public class PatientServiceTests
    {
        private readonly IPatientRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly PatientService _service;

        public PatientServiceTests()
        {
            _repo = Substitute.For<IPatientRepository>();
            _userRepo = Substitute.For<IUserRepository>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
            _service = new PatientService(_repo, _userRepo, _mapper);
        }

        private Patient MakePatient(int id = 1) => new()
        {
            Id = id,
            FirstName = "Ana",
            LastName = "Popova",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = "Female",
            PhoneNumber = "071234567",
            Email = "ana@test.com",
            Address = "Skopje",
            IsActive = true,
            Appointments = new List<Appointment>()
        };

        // ── Happy Path ────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAllAsync_CallsRepositoryAndReturnsMappedDtos()
        {
            // Arrange
            var patients = new List<Patient> { MakePatient(1), MakePatient(2) };
            _repo.GetAllAsync().Returns(patients);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            await _repo.Received(1).GetAllAsync();
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsDto_WhenPatientFound()
        {
            // Arrange
            var patient = MakePatient(1);
            _repo.GetByIdAsync(1).Returns(patient);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Ana", result.FirstName);
        }

        [Fact]
        public async Task CreateAsync_CallsRepositoryAndReturnsDto()
        {
            // Arrange
            var dto = new PatientCreateDto
            {
                FirstName = "Besa",
                LastName = "Leka",
                DateOfBirth = new DateTime(1995, 5, 5),
                Gender = "Female",
                PhoneNumber = "072345678",
                Email = "besa@test.com"
            };
            _repo.CreateAsync(Arg.Any<Patient>()).Returns(callInfo =>
            {
                var p = callInfo.Arg<Patient>();
                p.Id = 10;
                p.Appointments = new List<Appointment>();
                return p;
            });

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            await _repo.Received(1).CreateAsync(Arg.Any<Patient>());
            Assert.Equal(10, result.Id);
            Assert.Equal("Besa", result.FirstName);
        }

        // ── Sad Path ──────────────────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            _repo.GetByIdAsync(99).Returns((Patient?)null);

            // Act
            var result = await _service.GetByIdAsync(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmpty_WhenRepositoryReturnsEmpty()
        {
            // Arrange
            _repo.GetAllAsync().Returns(new List<Patient>());

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_WhenPatientNotFound()
        {
            // Arrange
            _repo.GetByIdAsync(99).Returns((Patient?)null);

            // Act
            var result = await _service.UpdateAsync(99, new PatientUpdateDto());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
        {
            // Arrange
            _repo.DeleteAsync(99).Returns(false);

            // Act
            var result = await _service.DeleteAsync(99);

            // Assert
            await _repo.Received(1).DeleteAsync(99);
            Assert.False(result);
        }
    }
}
