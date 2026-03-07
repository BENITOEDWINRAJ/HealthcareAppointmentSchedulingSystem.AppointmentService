using AppointmentService.Application.Commands;
using AppointmentService.Core.Entities;
using AppointmentService.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AppointmentService.Application.Handlers
{
    public class CreateAppointmentHandler
    {
        private readonly IAppointmentRepository _repo;
        private readonly ILogger<CreateAppointmentHandler> _logger;
        public CreateAppointmentHandler(IAppointmentRepository repo, ILogger<CreateAppointmentHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateAppointmentCommand command)
        {
            if (await _repo.HasOverlap(command.DoctorId, command.StartTime, command.EndTime))
                throw new Exception("Doctor has overlapping appointment");

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

            return appointment.Id;
        }
    }
}
