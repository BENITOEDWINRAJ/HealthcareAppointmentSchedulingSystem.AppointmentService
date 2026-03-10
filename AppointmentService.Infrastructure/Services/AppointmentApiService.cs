using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using AppointmentService.Application.Commands;
using System.Text.Json;
using AppointmentService.Application.Interfaces;

namespace AppointmentService.Infrastructure.Services
{
    public class AppointmentApiService : IAppointmentApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public AppointmentApiService(HttpClient httpClient,
            IHttpContextAccessor contextAccessor)
        {
            _httpClient = httpClient;
            _contextAccessor = contextAccessor;
        }

        private void AddJwtToken()
        {
            var token = _contextAccessor.HttpContext?
                .Request.Headers["Authorization"]
                .ToString();

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    AuthenticationHeaderValue.Parse(token);
            }
        }

        // 1️⃣ Get My Appointments
        public async Task<string> GetMyAppointments()
        {
            AddJwtToken();

            var response = await _httpClient.GetAsync("/api/appointments");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        // 2️⃣ Create Appointment
        public async Task<string> CreateAppointment(CreateAppointmentCommand command)
        {
            AddJwtToken();

            var json = JsonSerializer.Serialize(command);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                "/api/appointments",
                content);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        // 3️⃣ Search Appointments
        public async Task<string> SearchAppointments(
            Guid doctorId,
            DateTime start,
            DateTime end)
        {
            AddJwtToken();

            var url =
                $"/api/appointments/search?doctorId={doctorId}&start={start:o}&end={end:o}";

            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
