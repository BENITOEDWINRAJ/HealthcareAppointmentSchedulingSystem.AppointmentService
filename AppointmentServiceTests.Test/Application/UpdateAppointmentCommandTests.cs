using AppointmentService.Application.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentServiceTests.Test.Application
{
    public class UpdateAppointmentCommandTests
    {
        [Fact]
        public void UpdateAppointmentCommand_Should_Set_All_Properties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var patientId = Guid.NewGuid();
            var doctorId = Guid.NewGuid();
            var startTime = DateTime.UtcNow;
            var endTime = startTime.AddHours(1);
            var status = "Pending";

            // Act
            var command = new UpdateAppointmentCommand
            {
                Id = id,
                PatientId = patientId,
                DoctorId = doctorId,
                StartTime = startTime,
                EndTime = endTime,
                Status = status
            };

            // Assert
            Assert.Equal(id, command.Id);
            Assert.Equal(patientId, command.PatientId);
            Assert.Equal(doctorId, command.DoctorId);
            Assert.Equal(startTime, command.StartTime);
            Assert.Equal(endTime, command.EndTime);
            Assert.Equal(status, command.Status);
        }

        [Fact]
        public void UpdateAppointmentCommand_Status_Should_Not_Be_Null_Or_Empty()
        {
            // Arrange
            var command = new UpdateAppointmentCommand
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                DoctorId = Guid.NewGuid(),
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1),
                Status = "Confirmed"
            };

            // Assert
            Assert.False(string.IsNullOrEmpty(command.Status));
        }

        [Fact]
        public void UpdateAppointmentCommand_StartTime_Should_Be_Before_EndTime()
        {
            // Arrange
            var startTime = DateTime.UtcNow;
            var endTime = startTime.AddHours(1);

            var command = new UpdateAppointmentCommand
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                DoctorId = Guid.NewGuid(),
                StartTime = startTime,
                EndTime = endTime,
                Status = "Pending"
            };

            // Assert
            Assert.True(command.StartTime < command.EndTime);
        }
    }
}
