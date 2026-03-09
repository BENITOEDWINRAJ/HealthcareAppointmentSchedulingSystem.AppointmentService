using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Commands
{
    public class CreateAppointmentCommand
    {
        public Guid PatientId { get; set; }

        public Guid DoctorId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public required string Status { get; set; }
    }
}
