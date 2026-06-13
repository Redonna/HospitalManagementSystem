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
    public class DoctorServiceTests
    {
        private readonly IDoctorRepository _repo;
        private readonly IMapper _mapper;
        private readonly DoctorService _service;

        public DoctorServiceTests()
        {
            _repo = Substitute.For<IDoctorRepository>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
            _service = new DoctorService(_repo, _mapper);
        }

        private Doctor MakeDoctor(int id = 1) => new()
        {
            Id = id,
            FirstName = "James",
            LastName = "Smith",
            Specialization = "Cardiology",
            PhoneNumber = "071234567",
            Email = "james.smith@hospital.com",
            Department = "Cardiology",
            IsActive = true,
            Appointments = new List<Appointment>()
        };

        // ── Happy Path ────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAllAsync_CallsRepositoryAndReturnsMappedDtos()
        {
            // Arrange
            var doctors = new List<Doctor> { MakeDoctor(1), MakeDoctor(2) };
            _repo.GetAllAsync().Returns(doctors);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            await _repo.Received(1).GetAllAsync();
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsDto_WhenDoctorFound()
        {
            // Arrange
            var doctor = MakeDoctor(1);
            _repo.GetByIdAsync(1).Returns(doctor);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("James", result.FirstName);
            Assert.Equal("Cardiology", result.Specialization);
        }

        [Fact]
        public async Task CreateAsync_CallsRepositoryAndReturnsDto()
        {
            // Arrange
            var dto = new DoctorCreateDto
            {
                FirstName = "Sarah",
                LastName = "Connor",
                Specialization = "Neurology",
                PhoneNumber = "072345678",
                Email = "sarah.connor@hospital.com",
                Department = "Neurology"
            };
            _repo.CreateAsync(Arg.Any<Doctor>()).Returns(callInfo =>
            {
                var d = callInfo.Arg<Doctor>();
                d.Id = 10;
                d.Appointments = new List<Appointment>();
                return d;
            });

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            await _repo.Received(1).CreateAsync(Arg.Any<Doctor>());
            Assert.Equal(10, result.Id);
            Assert.Equal("Sarah", result.FirstName);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsUpdatedDto_WhenDoctorFound()
        {
            // Arrange
            var existing = MakeDoctor(1);
            _repo.GetByIdAsync(1).Returns(existing);
            _repo.UpdateAsync(1, Arg.Any<Doctor>()).Returns(callInfo =>
            {
                var d = callInfo.Arg<Doctor>();
                d.Appointments = new List<Appointment>();
                return d;
            });
            var updateDto = new DoctorUpdateDto { Specialization = "Oncology" };

            // Act
            var result = await _service.UpdateAsync(1, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Oncology", result.Specialization);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenDoctorDeleted()
        {
            // Arrange
            _repo.DeleteAsync(1).Returns(true);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            await _repo.Received(1).DeleteAsync(1);
            Assert.True(result);
        }

        // ── Sad Path ──────────────────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            _repo.GetByIdAsync(99).Returns((Doctor?)null);

            // Act
            var result = await _service.GetByIdAsync(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmpty_WhenRepositoryReturnsEmpty()
        {
            // Arrange
            _repo.GetAllAsync().Returns(new List<Doctor>());

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_WhenDoctorNotFound()
        {
            // Arrange
            _repo.GetByIdAsync(99).Returns((Doctor?)null);

            // Act
            var result = await _service.UpdateAsync(99, new DoctorUpdateDto());

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
