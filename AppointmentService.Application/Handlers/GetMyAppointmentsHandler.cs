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
namespace AppointmentService.Application.Handlers
{
    public class GetMyAppointmentsHandler
    {
        private readonly IAppointmentRepository _repo;
        private readonly ILogger<CreateAppointmentHandler> _logger;

        public GetMyAppointmentsHandler(IAppointmentRepository repo, ILogger<CreateAppointmentHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<IEnumerable<Appointment>> Handle(GetMyAppointmentsQuery query)
        {
            _logger.LogInformation("GetMyAppointmentsHandler Hadle method expect PatientId");
            return await _repo.GetByPatientIdAsync(query.PatientId);
            
        }
    }
}
