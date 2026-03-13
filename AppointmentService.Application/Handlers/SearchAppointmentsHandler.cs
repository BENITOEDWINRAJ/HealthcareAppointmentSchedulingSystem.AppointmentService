
using AppointmentService.Application.Handlers.Interfaces;
using AppointmentService.Application.Queries;
using AppointmentService.Core.Entities;
using AppointmentService.Core.Repositories;
using AppointmentService.Core.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Handlers
{
    public class SearchAppointmentsHandler : ISearchAppointmentsHandler
    {
        private readonly IAppointmentRepository _repo;

        public SearchAppointmentsHandler(IAppointmentRepository repo)
        {
            _repo = repo;
        }        
        public async Task<PagedResult<Appointment>> Handle(SearchAppointmentsQuery query)
        {
            return await _repo.SearchAsync(
                query.DoctorId,
                query.Start,
                query.End,
                query.Page,
                query.PageSize);
        }
    }
}
