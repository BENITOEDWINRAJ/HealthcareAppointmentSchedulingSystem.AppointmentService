using AppointmentService.Core.Entities;
using AppointmentService.Core.Repositories;
using AppointmentService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<Appointment>> GetByPatientIdAsync(Guid patientId)
        {
            return await _context.Appointments
                .Where(x => x.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<List<Appointment>> SearchAsync(Guid doctorId, DateTime start, DateTime end)
        {
            return await _context.Appointments
            .Where(a => a.DoctorId == doctorId
            && a.StartTime < end
            && a.EndTime > start)
            .OrderBy(a => a.StartTime)
            .ToListAsync();
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
    }
}









