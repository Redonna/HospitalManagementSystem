using HospitalManagementSystem.API.Controllers;
using HospitalManagementSystem.API.DTOs;
using HospitalManagementSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace HospitalManagementSystem.Tests.Controllers
{
    public class DoctorsControllerTests
    {
        private readonly IDoctorService _service;
        private readonly DoctorsController _controller;

        public DoctorsControllerTests()
        {
            _service = Substitute.For<IDoctorService>();
            _controller = new DoctorsController(_service);
        }

        private DoctorReadDto MakeDto(int id = 1) => new()
        {
            Id = id,
            FirstName = "James",
            LastName = "Smith",
            Specialization = "Cardiology",
            PhoneNumber = "071234567",
            Email = "james.smith@hospital.com",
            Department = "Cardiology"
        };

        // ── GET /api/doctors ──────────────────────────────────────────────────

        [Fact]
        public async Task GetAll_Returns200_WithDoctors()
        {
            // Arrange
            _service.GetAllAsync().Returns(new List<DoctorReadDto> { MakeDto(1), MakeDto(2) });

            // Act
            var result = await _controller.GetAll();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<IEnumerable<DoctorReadDto>>(ok.Value);
            Assert.Equal(2, data.Count());
        }

        // ── GET /api/doctors/{id} ─────────────────────────────────────────────

        [Fact]
        public async Task GetById_Returns200_WhenFound()
        {
            // Arrange
            _service.GetByIdAsync(1).Returns(MakeDto(1));

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsType<DoctorReadDto>(ok.Value);
            Assert.Equal(1, data.Id);
        }

        [Fact]
        public async Task GetById_Returns404_WhenNotFound()
        {
            // Arrange
            _service.GetByIdAsync(99).Returns((DoctorReadDto?)null);

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

        // ── POST /api/doctors ─────────────────────────────────────────────────

        [Fact]
        public async Task Create_Returns201_WhenValid()
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
            _service.CreateAsync(dto).Returns(MakeDto(5));

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, created.StatusCode);
        }

        // ── DELETE /api/doctors/{id} ──────────────────────────────────────────

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
