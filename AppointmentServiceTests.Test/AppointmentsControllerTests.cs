using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AppointmentService.API.Controllers;
using AppointmentService.Application.Commands;
using AppointmentService.Application.Handlers.Interfaces;
using AppointmentService.Application.Queries;
using AppointmentService.Core.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xunit;

namespace AppointmentServiceTests.Test
{
    public class AppointmentsControllerTests
    {
        private readonly Mock<IGetMyAppointmentsHandler> _getHandlerMock;
        private readonly Mock<ISearchAppointmentsHandler> _searchHandlerMock;
        private readonly Mock<ICreateAppointmentHandler> _createHandlerMock;
        private readonly Mock<ILogger<AppointmentsController>> _loggerMock;

        private readonly AppointmentsController _controller;

        public AppointmentsControllerTests()
        {
            _getHandlerMock = new Mock<IGetMyAppointmentsHandler>();
            _searchHandlerMock = new Mock<ISearchAppointmentsHandler>();
            _createHandlerMock = new Mock<ICreateAppointmentHandler>();
            _loggerMock = new Mock<ILogger<AppointmentsController>>();

            _controller = new AppointmentsController(
                _getHandlerMock.Object,
                _searchHandlerMock.Object,
                _createHandlerMock.Object,
                _loggerMock.Object);
        }

        // -----------------------------------------
        // Test: GetMyAppointments - Success
        // -----------------------------------------
        [Fact]
        public async Task GetMyAppointments_ReturnsOk_WithAppointments()
        {
            var patientId = Guid.NewGuid();

            var claims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
             new Claim(ClaimTypes.NameIdentifier, patientId.ToString())
            }));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext { User = claims }
            };

            var expectedResult = new List<Appointment>();

            _getHandlerMock
                .Setup(x => x.Handle(It.IsAny<GetMyAppointmentsQuery>()))
                .ReturnsAsync(expectedResult);

            var result = await _controller.GetMyAppointments();

            var okResult = result as OkObjectResult;

            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
        }

        // -----------------------------------------
        // Test: GetMyAppointments - Unauthorized
        // -----------------------------------------
        [Fact]
        public async Task GetMyAppointments_ReturnsUnauthorized_WhenClaimMissing()
        {
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await _controller.GetMyAppointments();

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        // -----------------------------------------
        // Test: Create Appointment
        // -----------------------------------------
        [Fact]
        public async Task Create_ReturnsOk_WithAppointmentId()
        {
            var appointmentId = Guid.NewGuid();

            var command = new CreateAppointmentCommand
            {
                PatientId = Guid.NewGuid(),
                DoctorId = Guid.NewGuid(),
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1),
                Status = "Scheduled"
            };

            _createHandlerMock
                .Setup(x => x.Handle(It.IsAny<CreateAppointmentCommand>()))
                .ReturnsAsync(appointmentId);

            var result = await _controller.Create(command);

            var okResult = result as OkObjectResult;

            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(appointmentId);
        }

        // -----------------------------------------
        // Test: Search Appointments
        // -----------------------------------------
        [Fact]
        public async Task Search_ReturnsOk_WithResults()
        {
            var doctorId = Guid.NewGuid();

            var expected = new List<Appointment>();

            _searchHandlerMock
                .Setup(x => x.Handle(It.IsAny<SearchAppointmentsQuery>()))
                .ReturnsAsync(expected);

            var result = await _controller.Search(
                doctorId,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(1));

            var okResult = result as OkObjectResult;

            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
        }
    }
}
