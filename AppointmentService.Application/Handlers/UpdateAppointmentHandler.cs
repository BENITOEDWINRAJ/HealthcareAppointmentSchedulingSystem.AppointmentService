using AppointmentService.Application.Commands;
using AppointmentService.Application.Handlers.Interfaces;
using AppointmentService.Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Handlers
{
    public class UpdateAppointmentHandler :IUpdateAppointmentHandler
    {
        private readonly IAppointmentRepository _repository;

        public UpdateAppointmentHandler(IAppointmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(Guid id,
        [FromBody] UpdateAppointmentCommand command)
        {
            var appointment = await _repository.GetByIdAsync(id);            

            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found");

            var overlap = await _repository.HasOverlap(
                command.DoctorId,
                command.StartTime,
                command.EndTime);

            if (overlap)
                throw new Exception("Doctor already has appointment in this slot");

            appointment.DoctorId = command.DoctorId;
            appointment.PatientId = command.PatientId;
            appointment.StartTime = command.StartTime;
            appointment.EndTime = command.EndTime;
            appointment.Status = command.Status;

            await _repository.UpdateAsync(appointment);

            return true;
        }
    }
}
