using AppointmentService.Application.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Handlers.Interfaces
{
    public interface ICreateAppointmentHandler
    {
        Task<Guid> Handle(CreateAppointmentCommand command);
    }
}
