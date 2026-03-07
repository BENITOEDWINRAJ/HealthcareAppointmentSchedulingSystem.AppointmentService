using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Queries
{
    public class GetMyAppointmentsQuery
    {
        public Guid PatientId { get; set; }

        public GetMyAppointmentsQuery(Guid patientId)
        {
            PatientId = patientId;
        }
    }
}
