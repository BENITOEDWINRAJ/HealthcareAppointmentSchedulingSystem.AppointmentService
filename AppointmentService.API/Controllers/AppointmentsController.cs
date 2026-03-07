using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AppointmentService.Application.Commands;
using AppointmentService.Core.Entities;
using AppointmentService.Core.Repositories;
using AppointmentService.Application.Handlers;
using AppointmentService.Application.Queries;

namespace AppointmentService.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly GetMyAppointmentsHandler _getHandler;
        private readonly SearchAppointmentsHandler _searchHandler;
        private readonly CreateAppointmentHandler _createHandler;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(
            GetMyAppointmentsHandler getHandler,
            SearchAppointmentsHandler searchHandler,
            CreateAppointmentHandler createHandler,
            ILogger<AppointmentsController> logger)
        {
            _getHandler = getHandler;
            _searchHandler = searchHandler;
            _createHandler = createHandler;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyAppointments()
        {
            var patientIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(patientIdStr))
                return Unauthorized("User identifier claim missing");

            var query = new GetMyAppointmentsQuery(Guid.Parse(patientIdStr));
            _logger.LogInformation("GetMyAppointments Handlers process are started");
            var result = await _getHandler.Handle(query);
            _logger.LogInformation("GetMyAppointments Handlers process are End");

            return Ok(result);
            /*var patientIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(patientIdStr))
                return Unauthorized("User identifier claim is missing.");

            var patientId = Guid.Parse(patientIdStr);

            _logger.LogInformation("Get appointment detaild for patient");

            var appointments = await _repo.GetByPatientIdAsync(patientId);

            _logger.LogInformation("Collected appointment details successfully");

            return Ok(appointments);*/
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAppointmentCommand command)
        {
            _logger.LogInformation("Create Handlers process are started");
            var id = await _createHandler.Handle(command);
            _logger.LogInformation("Create Handlers process are end");
            return Ok(id);
            /*if (await _repo.HasOverlap(command.DoctorId, command.StartTime, command.EndTime))
                return BadRequest("Doctor has overlapping appointment");

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = command.PatientId,
                DoctorId = command.DoctorId,
                StartTime = command.StartTime,
                EndTime = command.EndTime
            };

            _logger.LogInformation(
        "Creating appointment for patient {PatientId} with doctor {DoctorId}",
        appointment.PatientId,
        appointment.DoctorId);

            await _repo.AddAsync(appointment);

            _logger.LogInformation("Appointment created successfully");

            return Ok(appointment.Id);*/
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(Guid doctorId, DateTime start, DateTime end)
        {
            var query = new SearchAppointmentsQuery
            {
                DoctorId = doctorId,
                Start = start,
                End = end
            };

            _logger.LogInformation("Search Handlers process are started");
            var result = await _searchHandler.Handle(query);
            _logger.LogInformation("Search Handlers process are end");

            return Ok(result);
            /*_logger.LogInformation("starting to the search process");
            var result = await _repo.SearchAsync(doctorId, start, end);
            _logger.LogInformation("End to the search process");

            return Ok(result);*/
        }
    }
}
