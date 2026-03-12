using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Commands
{
    public class DeleteAppointmentCommand
    {
        public Guid Id { get; set; }
    }
}
