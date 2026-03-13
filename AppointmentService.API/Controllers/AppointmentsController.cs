using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AppointmentService.Application.Commands;
using AppointmentService.Core.Entities;
using AppointmentService.Core.Repositories;
using AppointmentService.Application.Handlers;
using AppointmentService.Application.Handlers.Interfaces;
using AppointmentService.Application.Queries;
using Microsoft.EntityFrameworkCore;
using AppointmentService.Infrastructure.Services;
using AppointmentService.Application.DTOs;

namespace AppointmentService.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IGetMyAppointmentsHandler _getHandler;
        private readonly ISearchAppointmentsHandler _searchHandler;
        private readonly ICreateAppointmentHandler _createHandler;
        private readonly ILogger<AppointmentsController> _logger;
        private readonly IUpdateAppointmentHandler _updateHandler;
        private readonly IDeleteAppointmentHandler _deleteHandler;
        private readonly IJwtService _jwt;
        private readonly PatientServiceClient _patientClient;

        public AppointmentsController(
            IGetMyAppointmentsHandler getHandler,
            ISearchAppointmentsHandler searchHandler,
            ICreateAppointmentHandler createHandler,
            ILogger<AppointmentsController> logger,
            IUpdateAppointmentHandler updateHandler,
            IDeleteAppointmentHandler deleteHandler,
            IJwtService jwt,
            PatientServiceClient patientClient)
        {
            _getHandler = getHandler;
            _searchHandler = searchHandler;
            _createHandler = createHandler;
            _logger = logger;
            _updateHandler = updateHandler;
            _deleteHandler = deleteHandler;
            _jwt = jwt;
            _patientClient = patientClient;
        }
        [Authorize(Roles = "Patient")]
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
            
        }

        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentCommand command)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation("Create Handlers process are started");
            var id = await _createHandler.Handle(command);
            _logger.LogInformation("Create Handlers process are end");
            return Ok(new
            {
                LoggedInUserId = userId,
                Username = username,
                Role = role,
                data = id,
                Message = "Appointment created successfully"
            });
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("search")]
        public async Task<IActionResult> Search(DateTime start, DateTime end)
        {
            /*var query = new SearchAppointmentsQuery
            {
                DoctorId = doctorId,
                Start = start,
                End = end
            };

            _logger.LogInformation("Search Handlers process are started");
            var result = await _searchHandler.Handle(query);
            _logger.LogInformation("Search Handlers process are end");

            return Ok(result);  */
            var doctorId = Guid.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

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
        }

        // UPDATE APPOINTMENT
       // [Authorize(Roles = "Doctor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
    Guid id,
    [FromBody] UpdateAppointmentCommand command)
        {
            if (id != command.Id)
                return BadRequest("Appointment ID mismatch");

            try
            {
                var result = await _updateHandler.Handle(id,command);
                return Ok(new
                {
                    Message = "Appointment deleted successfully",
                    Data = result
                }); 
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Return 400 for known domain errors, or 500 for unexpected
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE APPOINTMENT
       // [Authorize(Roles = "Doctor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteAppointmentCommand
            {
                Id = id
            };

            var result = await _deleteHandler.Handle(id);

            return Ok(new
            {
                Message = "Appointment deleted successfully",
                Data = result
            });
        }
        [Authorize(Roles ="Doctor")]
        [HttpGet("AllRegisteredUsers")]
        public async Task<List<UserDto>> GetAllRegisters()
        {
            var users = await _patientClient.GetUsers();

            return (users);
        }
        /* [Authorize(Roles = "Doctor")]
         [HttpPut("{id}/status")]
         public async Task<IActionResult> UpdateStatus(Guid id, string status)
         {
             /*var appointment = await _context.Appointments.FindAsync(id);
             appointment.Status = status;

             await _context.SaveChangesAsync();

             return Ok();

         }*/
    }
}
