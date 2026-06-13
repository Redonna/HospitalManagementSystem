using HospitalManagementSystem.API.Controllers;
using HospitalManagementSystem.API.DTOs;
using HospitalManagementSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace HospitalManagementSystem.Tests.Controllers
{
    public class PatientsControllerTests
    {
        private readonly IPatientService _service;
        private readonly PatientsController _controller;

        public PatientsControllerTests()
        {
            _service = Substitute.For<IPatientService>();
            _controller = new PatientsController(_service);
        }

        private PatientReadDto MakeDto(int id = 1) => new()
        {
            Id = id,
            FirstName = "Ana",
            LastName = "Popova",
            Email = "ana@test.com",
            Gender = "Female",
            PhoneNumber = "071234567",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        // ── GET /api/patients ─────────────────────────────────────────────────

        [Fact]
        public async Task GetAll_Returns200_WithPatients()
        {
            // Arrange
            _service.GetAllAsync().Returns(new List<PatientReadDto> { MakeDto(1), MakeDto(2) });

            // Act
            var result = await _controller.GetAll();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<IEnumerable<PatientReadDto>>(ok.Value);
            Assert.Equal(2, data.Count());
        }

        // ── GET /api/patients/{id} ────────────────────────────────────────────

        [Fact]
        public async Task GetById_Returns200_WhenFound()
        {
            // Arrange
            _service.GetByIdAsync(1).Returns(MakeDto(1));

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsType<PatientReadDto>(ok.Value);
            Assert.Equal(1, data.Id);
        }

        [Fact]
        public async Task GetById_Returns404_WhenNotFound()
        {
            // Arrange
            _service.GetByIdAsync(99).Returns((PatientReadDto?)null);

            // Act
            var result = await _controller.GetById(99);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetById_Returns400_WhenIdIsInvalid()
        {
            // Act
            var result = await _controller.GetById(0);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        // ── POST /api/patients ────────────────────────────────────────────────

        [Fact]
        public async Task Create_Returns201_WhenValid()
        {
            // Arrange
            var dto = new PatientCreateDto
            {
                FirstName = "Drita",
                LastName = "Meka",
                DateOfBirth = new DateTime(1992, 3, 3),
                Gender = "Female",
                PhoneNumber = "073456789",
                Email = "drita@test.com"
            };
            _service.CreateAsync(dto).Returns(MakeDto(5));

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, created.StatusCode);
        }

        // ── DELETE /api/patients/{id} ─────────────────────────────────────────

        [Fact]
        public async Task Delete_Returns204_WhenDeleted()
        {
            // Arrange
            _service.DeleteAsync(1).Returns(true);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_Returns404_WhenNotFound()
        {
            // Arrange
            _service.DeleteAsync(99).Returns(false);

            // Act
            var result = await _controller.Delete(99);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
