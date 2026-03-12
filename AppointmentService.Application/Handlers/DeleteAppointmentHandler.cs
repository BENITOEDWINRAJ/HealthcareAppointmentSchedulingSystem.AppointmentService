using AppointmentService.Application.Commands;
using AppointmentService.Application.Handlers.Interfaces;
using AppointmentService.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Handlers
{
    public class DeleteAppointmentHandler :IDeleteAppointmentHandler
    {
        private readonly IAppointmentRepository _repository;

        public DeleteAppointmentHandler(IAppointmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(Guid id)
        {
            var appointment = await _repository.GetByIdAsync(id);

            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found");

            await _repository.DeleteAsync(id);

            return true;
        }
    }
}
