using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerReader
{
    public class Worker : BackgroundService
    {

        public Worker()
        {

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "barreto", Password = "barretoPass" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "fila01",
                                    durable: true,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                while (!stoppingToken.IsCancellationRequested)
                {
                    consumer.Received += ConsumerReceived;

                    channel.BasicConsume(queue: "fila01",
                                         autoAck: true,
                                         consumer: consumer);
                }

                await Task.Delay(1000, stoppingToken);
            }
        }

        private void ConsumerReceived(object sender, BasicDeliverEventArgs e)
        {            
            Console.WriteLine(
                $"[Nova mensagem | {DateTime.Now:yyyy-MM-dd HH:mm:ss}] " +
                Encoding.UTF8.GetString(e.Body.ToArray())
            );
        }
    }
}
