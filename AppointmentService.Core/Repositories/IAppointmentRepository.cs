using AppointmentService.Core.Entities;
using AppointmentService.Core.Common;

using AppointmentService;
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
        
        Task<PagedResult<Appointment>> SearchAsync(Guid doctorId,DateTime start,DateTime end,int page,int pageSize);        

        Task<bool> HasOverlap(Guid doctorId, DateTime start, DateTime end);

        Task<List<Appointment>> GetPagedAsync(int page, int size);
        Task UpdateAsync(Appointment appointment);
        Task DeleteAsync(Guid id);
        Task<Appointment> GetByIdAsync(Guid id);
    }
}
