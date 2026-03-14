using AppointmentService.Application.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentServiceTests.Test.Application
{
    public class DeleteAppointmentCommandTests
    {
        [Fact]
        public void DeleteAppointmentCommand_Should_Set_Id()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();

            // Act
            var command = new DeleteAppointmentCommand
            {
                Id = appointmentId
            };

            // Assert
            Assert.Equal(appointmentId, command.Id);
        }

        [Fact]
        public void DeleteAppointmentCommand_Id_Should_Not_Be_Empty()
        {
            // Arrange
            var command = new DeleteAppointmentCommand
            {
                Id = Guid.NewGuid()
            };

            // Assert
            Assert.NotEqual(Guid.Empty, command.Id);
        }
    }
}
