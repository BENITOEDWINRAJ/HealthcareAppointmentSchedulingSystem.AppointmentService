using AppointmentService.Application.Commands;
using AppointmentService.Core.Entities;
using AppointmentService.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AppointmentService.Application.Handlers.Interfaces;
using AppointmentService.Application.Common.Interfaces;

namespace AppointmentService.Application.Handlers
{
    public class CreateAppointmentHandler : ICreateAppointmentHandler
    {
        private readonly IAppointmentRepository _repo;
        private readonly ILogger<CreateAppointmentHandler> _logger;
        private readonly IPatientServiceClient _patientServiceClient;
        public CreateAppointmentHandler(IAppointmentRepository repo, ILogger<CreateAppointmentHandler> logger, IPatientServiceClient patientServiceClient)
        {
            _repo = repo;
            _logger = logger;
            _patientServiceClient = patientServiceClient;
        }

        public async Task<Guid> Handle(CreateAppointmentCommand command, string userId, string role)
        {
            if (role == "Patient")
            {
                if (command.PatientId.ToString() != userId)
                {
                    throw new UnauthorizedAccessException(
                        "Patients can create appointments only for themselves.");
                }
            }

            // Validate Patient from PatientService
            var users = await _patientServiceClient.GetUsers();

            var patientExists = users.Any(u =>
                u.Id == command.PatientId && u.Role == "Patient");

            if (!patientExists)
                throw new Exception("Invalid PatientId. Patient does not exist.");

            if (command.StartTime >= command.EndTime)
                throw new ArgumentException("StartTime must be before EndTime");

            var hasOverlap = await _repo.HasOverlap(
                command.DoctorId,
                command.StartTime,
                command.EndTime);

            if (hasOverlap)
                throw new Exception("Doctor has overlapping appointment");

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = command.PatientId,
                DoctorId = command.DoctorId,
                StartTime = command.StartTime,
                EndTime = command.EndTime,
                Status = command.Status
            };

            await _repo.AddAsync(appointment);

            return appointment.Id;
        }
    }
}
