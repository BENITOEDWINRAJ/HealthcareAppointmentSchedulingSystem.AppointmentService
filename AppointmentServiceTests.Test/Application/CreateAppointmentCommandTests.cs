using AppointmentService.Application.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AppointmentServiceTests.Test.Application
{
    public class CreateAppointmentCommandTests
    {
        [Fact]
        public void CreateAppointmentCommand_Should_Set_All_Properties()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var doctorId = Guid.NewGuid();
            var startTime = DateTime.UtcNow;
            var endTime = startTime.AddHours(1);
            var status = "Pending";

            // Act
            var command = new CreateAppointmentCommand
            {
                PatientId = patientId,
                DoctorId = doctorId,
                StartTime = startTime,
                EndTime = endTime,
                Status = status
            };

            // Assert
            Assert.Equal(patientId, command.PatientId);
            Assert.Equal(doctorId, command.DoctorId);
            Assert.Equal(startTime, command.StartTime);
            Assert.Equal(endTime, command.EndTime);
            Assert.Equal(status, command.Status);
        }

        [Fact]
        public void CreateAppointmentCommand_StartTime_Should_Be_Before_EndTime()
        {
            // Arrange
            var startTime = DateTime.UtcNow;
            var endTime = startTime.AddHours(1);

            var command = new CreateAppointmentCommand
            {
                PatientId = Guid.NewGuid(),
                DoctorId = Guid.NewGuid(),
                StartTime = startTime,
                EndTime = endTime,
                Status = "Pending"
            };

            // Assert
            Assert.True(command.StartTime < command.EndTime);
        }

        [Fact]
        public void CreateAppointmentCommand_Status_Should_Not_Be_Null()
        {
            // Arrange
            var command = new CreateAppointmentCommand
            {
                PatientId = Guid.NewGuid(),
                DoctorId = Guid.NewGuid(),
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1),
                Status = "Confirmed"
            };

            // Assert
            Assert.False(string.IsNullOrEmpty(command.Status));
        }
    }
}
