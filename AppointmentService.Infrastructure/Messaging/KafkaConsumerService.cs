using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using System.ComponentModel;
using SharedKafka.Events;
namespace AppointmentService.Infrastructure.Messaging
{
    public class KafkaConsumerService :BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:7001",
                GroupId = "appointment-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            var consumer = new ConsumerBuilder<string, string>(config).Build();            

            try
            {
                consumer.Subscribe("user-created-topic");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var result = consumer.Consume(stoppingToken);

                    var userEvent = JsonSerializer.Deserialize<UserCreatedEvent>(
                        result.Message.Value);

                    Console.WriteLine($"User received: {userEvent.Username}");
                }
            });
        }

    }
}
