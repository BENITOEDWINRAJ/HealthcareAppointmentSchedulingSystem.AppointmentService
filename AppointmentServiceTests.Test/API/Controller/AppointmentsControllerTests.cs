using AppointmentService.API.Controllers;
using AppointmentService.Application.Commands;
using AppointmentService.Application.Common.Interfaces;
using AppointmentService.Application.Handlers.Interfaces;
using AppointmentService.Application.Queries;
using AppointmentService.Core.Common;
using AppointmentService.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentServiceTests.Test.API.Controller
{
    public class AppointmentsControllerTests
    {
        private readonly Mock<IGetMyAppointmentsHandler> _getHandlerMock = new();
        private readonly Mock<ISearchAppointmentsHandler> _searchHandlerMock = new();
        private readonly Mock<ICreateAppointmentHandler> _createHandlerMock = new();
        private readonly Mock<IUpdateAppointmentHandler> _updateHandlerMock = new();
        private readonly Mock<IDeleteAppointmentHandler> _deleteHandlerMock = new();
        private readonly Mock<IJwtService> _jwtMock = new();
        private readonly Mock<IPatientServiceClient> _patientClientMock = new();
        private readonly Mock<ILogger<AppointmentsController>> _loggerMock = new();

        private AppointmentsController CreateController(string userId = "user1", string role = "Doctor")
        {
            var controller = new AppointmentsController(
                _getHandlerMock.Object,
                _searchHandlerMock.Object,
                _createHandlerMock.Object,
                _loggerMock.Object,
                _updateHandlerMock.Object,
                _deleteHandlerMock.Object,
                _jwtMock.Object,
                _patientClientMock.Object
            );

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim(ClaimTypes.Role, role)
        }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            return controller;
        }

        [Fact]
        public async Task GetAppointments_ReturnsOkResult()
        {
            var controller = CreateController();

            _getHandlerMock.Setup(x => x.Handle(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new List<Appointment>());

            var result = await controller.GetAppointments();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsOkResult()
        {
            var controller = CreateController();

            var command = new CreateAppointmentCommand
            {
                PatientId = Guid.NewGuid(),
                DoctorId = Guid.NewGuid(),
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1),
                Status = "Scheduled"
            };

            _createHandlerMock.Setup(x => x.Handle(It.IsAny<CreateAppointmentCommand>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(Guid.NewGuid());

            var result = await controller.Create(command);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

       [Fact]
        public async Task Search_ReturnsOkResult()
        {
            var controller = CreateController();

            var id = Guid.NewGuid();

            var command = new UpdateAppointmentCommand
            {
                Id = id,
                Status = "Pending"
            };

            _updateHandlerMock
                .Setup(x => x.Handle(It.IsAny<Guid>(), It.IsAny<UpdateAppointmentCommand>()))
                .ReturnsAsync(true);

            var result = await controller.Update(id, command);

            var okResult = Assert.IsType<OkObjectResult>(result);

            _updateHandlerMock.Verify(
                x => x.Handle(id, command),
                Times.Once);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch()
        {
            var controller = CreateController();

            var command = new UpdateAppointmentCommand
            {
                Id = Guid.NewGuid(),
                Status = "Completed"
            };

            var result = await controller.Update(Guid.NewGuid(), command);

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var controller = CreateController();

            var id = Guid.NewGuid();

            var command = new UpdateAppointmentCommand
            {
                Id = id,              
                Status = "Pending"
            };

            _updateHandlerMock
                .Setup(x => x.Handle(It.IsAny<Guid>(), It.IsAny<UpdateAppointmentCommand>()))
                .ReturnsAsync(true);

            // Act
            var result = await controller.Update(id, command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            _updateHandlerMock.Verify(
                x => x.Handle(id, command),
                Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnsOkResult()
        {
            var controller = CreateController();

            var id = Guid.NewGuid();

            _deleteHandlerMock.Setup(x => x.Handle(id))
                .ReturnsAsync(true);

            var result = await controller.Delete(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenAppointmentDoesNotExist()
        {
            var controller = CreateController();

            var id = Guid.NewGuid();

            var command = new UpdateAppointmentCommand
            {
                Id = id,
                Status = "Pending"
            };

            _updateHandlerMock
                .Setup(x => x.Handle(id, command))
                .ThrowsAsync(new KeyNotFoundException("Appointment not found"));

            var result = await controller.Update(id, command);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
