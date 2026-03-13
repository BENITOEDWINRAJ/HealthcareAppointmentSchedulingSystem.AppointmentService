using AppointmentService.Application.DTOs;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AppointmentService.Infrastructure.Services
{
    public class PatientServiceClient
    {
        //private readonly HttpClient _httpClient;

        //public PatientServiceClient(HttpClient httpClient)
        //{
        //    _httpClient = httpClient;
        //}

        //public async Task<List<UserDto>> GetUsers()
        //{
        //    var users = await _httpClient
        //        .GetFromJsonAsync<List<UserDto>>("api/Auth/AllRegisteredUsers");

        //    return users ?? new List<UserDto>();
        //}
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PatientServiceClient(
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<UserDto>> GetUsers()
        {
            // Get JWT token from incoming request
            var token = _httpContextAccessor.HttpContext?
                .Request.Headers["Authorization"]
                .ToString();

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    AuthenticationHeaderValue.Parse(token);
            }

            var users = await _httpClient
                .GetFromJsonAsync<List<UserDto>>("api/Auth/AllRegisteredUsers");

            return users ?? new List<UserDto>();
        }
    }
}