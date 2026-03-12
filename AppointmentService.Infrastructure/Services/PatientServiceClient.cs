using AppointmentService.Application.DTOs;
using System.Net.Http.Json;

namespace AppointmentService.Infrastructure.Services
{
    public class PatientServiceClient
    {
        private readonly HttpClient _httpClient;

        public PatientServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<UserDto>> GetUsers()
        {
            var users = await _httpClient
                .GetFromJsonAsync<List<UserDto>>("api/Auth/users");

            return users ?? new List<UserDto>();
        }
    }
}