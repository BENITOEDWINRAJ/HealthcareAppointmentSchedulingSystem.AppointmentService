using AppointmentService.Application.Queries;
using AppointmentService.Core.Common;
using AppointmentService.Core.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Handlers.Interfaces
{
    public interface ISearchAppointmentsHandler
    {
        Task<PagedResult<Appointment>> Handle(SearchAppointmentsQuery query);
    }
}
