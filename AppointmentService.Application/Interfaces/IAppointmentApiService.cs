using AppointmentService.Application.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Interfaces
{
    public interface IAppointmentApiService
    {
        public Task<string> GetMyAppointments();

        Task<string> CreateAppointment(CreateAppointmentCommand command);

        Task<string> SearchAppointments(Guid doctorId, DateTime start, DateTime end);
    }
}
