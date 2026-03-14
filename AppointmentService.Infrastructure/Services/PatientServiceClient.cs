using AppointmentService.Application.Common.Interfaces;
using AppointmentService.Application.DTOs;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppointmentService.Infrastructure.Services
{
    public class PatientServiceClient : IPatientServiceClient
    {

        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PatientServiceClient(HttpClient httpClient,
                                    IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<UserDto>> GetUsers()
        {
            var token = _httpContextAccessor.HttpContext?
                .Request.Headers["Authorization"]
                .ToString();

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    AuthenticationHeaderValue.Parse(token);
            }

            // Get raw JSON response
            var response = await _httpClient.GetAsync("api/Auth/AllRegisteredUsers");

            if (!response.IsSuccessStatusCode)
            {
                // Log and return empty list if API fails
                return new List<UserDto>();
            }

            var jsonString = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(jsonString))
                return new List<UserDto>();

            try
            {
                // Try deserialize as List<UserDto>
                var usersList = JsonSerializer.Deserialize<List<UserDto>>(jsonString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (usersList != null)
                    return usersList;

                // If deserialization returned null, try as single object
                var singleUser = JsonSerializer.Deserialize<UserDto>(jsonString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (singleUser != null)
                    return new List<UserDto> { singleUser };

                // Nothing found
                return new List<UserDto>();
            }
            catch (JsonException)
            {
                // If JSON is malformed, return empty list
                return new List<UserDto>();
            }
        }
    }
}