
using AppointmentService.Application.Queries;
using AppointmentService.Core.Entities;
using AppointmentService.Core.Repositories;
using AppointmentService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using AppointmentService.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Infrastructure.Repositories
{
    public class AppointmentRepository:IAppointmentRepository
    {
        private readonly AppointmentDbContext _context;

        public AppointmentRepository(AppointmentDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
        }       

        public async Task<List<Appointment>> GetAllAppointments()
        {
            return await _context.Appointments
                .OrderByDescending(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetAppointmentsByPatientId(Guid patientId)
        {
            return await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.StartTime)
                .ToListAsync();
        }


        public async Task<PagedResult<Appointment>> SearchAsync(
            Guid doctorId,
            DateTime start,
            DateTime end,
            int page,
            int pageSize)
        {
            var query = _context.Appointments
                .Where(a => a.DoctorId == doctorId &&
                            a.StartTime >= start &&
                            a.EndTime <= end);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(a => a.StartTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Appointment>(items, totalCount, page, pageSize);
        }

        public async Task<bool> HasOverlap(Guid doctorId, DateTime start, DateTime end)
        {
            return await _context.Appointments.AnyAsync(a =>
                a.DoctorId == doctorId &&
                start < a.EndTime &&
                end > a.StartTime);
        }

        public async Task<List<Appointment>> GetPagedAsync(int page, int size)
        {
            return await _context.Appointments
                .OrderBy(a => a.StartTime)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();
        }

        public async Task<Appointment> GetByIdAsync(Guid id)
        {
            return await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task DeleteAsync(Guid id)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found");

            _context.Appointments.Remove(appointment);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> PatientExists(Guid patientId)
        {
            return await _context.Appointments.AnyAsync(p => p.Id == patientId);
        }
    }
}









