using AppointmentService.Application.Commands;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Handlers.Interfaces
{
    public interface IUpdateAppointmentHandler
    {
        Task<bool> Handle(Guid id,
        [FromBody] UpdateAppointmentCommand command);
    }
}
