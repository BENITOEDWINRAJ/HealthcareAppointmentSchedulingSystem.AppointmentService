using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Queries
{
    public class SearchAppointmentsQuery
    {
        public Guid DoctorId { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }
    }
}
