using AppointmentService.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Core.Repositories
{
    public interface IAppointmentRepository
    {
        Task AddAsync(Appointment appointment);

        Task<List<Appointment>> GetByPatientIdAsync(Guid patientId);

        Task<List<Appointment>> SearchAsync(Guid doctorId, DateTime start, DateTime end);

        Task<bool> HasOverlap(Guid doctorId, DateTime start, DateTime end);

        Task<List<Appointment>> GetPagedAsync(int page, int size);
    }
}
