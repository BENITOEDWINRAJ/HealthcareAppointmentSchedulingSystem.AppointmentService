using AppointmentService.Application.Queries;
using AppointmentService.Core.Entities;
using AppointmentService.Core.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AppointmentService.Application.Handlers.Interfaces;
using System.Data;
namespace AppointmentService.Application.Handlers
{
    public class GetMyAppointmentsHandler : IGetMyAppointmentsHandler
    {
        private readonly IAppointmentRepository _repo;
        private readonly ILogger<GetMyAppointmentsHandler> _logger;

        public GetMyAppointmentsHandler(IAppointmentRepository repo, ILogger<GetMyAppointmentsHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<IEnumerable<Appointment>> Handle(string userId, string role)
        {
            if (string.IsNullOrEmpty(role))
                throw new UnauthorizedAccessException("Invalid token");

            List<Appointment> appointments;

            if (role == "Doctor")
            {
                // Doctor can view all appointments
                appointments = await _repo.GetAllAppointments();
            }
            else if (role == "Patient")
            {
                // Patient can view only his appointments
                appointments = await _repo.GetAppointmentsByPatientId(Guid.Parse(userId));
            }
            else
            {
                throw new UnauthorizedAccessException("Invalid role");
            }

            return appointments;
        }
    }

}
