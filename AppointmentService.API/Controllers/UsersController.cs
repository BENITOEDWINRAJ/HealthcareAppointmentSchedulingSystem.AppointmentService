using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppointmentService.Application.DTOs;
using AppointmentService.Application.Handlers.Interfaces;
using AppointmentService.Application.Common.Interfaces;
namespace AppointmentService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IGetMyAppointmentsHandler _getHandler;
        private readonly ISearchAppointmentsHandler _searchHandler;
        private readonly ICreateAppointmentHandler _createHandler;
        private readonly ILogger<AppointmentsController> _logger;
        private readonly IUpdateAppointmentHandler _updateHandler;
        private readonly IDeleteAppointmentHandler _deleteHandler;
        private readonly IJwtService _jwt;
        private readonly IPatientServiceClient _patientClient;

        public UsersController(
            IGetMyAppointmentsHandler getHandler,
            ISearchAppointmentsHandler searchHandler,
            ICreateAppointmentHandler createHandler,
            ILogger<AppointmentsController> logger,
            IUpdateAppointmentHandler updateHandler,
            IDeleteAppointmentHandler deleteHandler,
            IJwtService jwt,
            IPatientServiceClient patientClient)
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
        //  Get all registered users (for Doctor role only)
        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> GetAllRegisteredUsers()
        {
            var users = await _patientClient.GetUsers();

            return Ok(new
            {
                Data = users,
                Message = "Registered users retrieved successfully"
            });
        }
    }
}
