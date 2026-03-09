using AppointmentService.Application.Queries;
using AppointmentService.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Handlers.Interfaces
{
    public interface IGetMyAppointmentsHandler
    {
        Task<IEnumerable<Appointment>> Handle(GetMyAppointmentsQuery query);
    }
}
