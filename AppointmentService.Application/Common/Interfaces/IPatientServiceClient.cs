using AppointmentService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Common.Interfaces
{
    public interface IPatientServiceClient
    {
        Task<List<UserDto>> GetUsers();
    }
}
