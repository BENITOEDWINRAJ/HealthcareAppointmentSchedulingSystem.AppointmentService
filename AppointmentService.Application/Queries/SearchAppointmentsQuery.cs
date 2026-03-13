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

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 3;
    }
}
