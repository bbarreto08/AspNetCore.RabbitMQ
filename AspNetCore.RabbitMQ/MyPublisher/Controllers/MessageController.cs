using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyPublisher.Model;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace MyPublisher.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        [HttpPost]
        public IActionResult Send([FromBody] Message msg)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using(var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                string message = JsonConvert.SerializeObject(msg);
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "teste",
                                     routingKey: "key-a",
                                     basicProperties: null,
                                     body: body);
            }

            return Ok();
        }

        public IActionResult Read()
        {
            var message = "";

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "fila01",
                                    durable: true,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    message = Encoding.UTF8.GetString(body);

                    //Console.WriteLine(" [x] Received {0}", message);
                };

                channel.BasicConsume(queue: "fila01",
                                     autoAck: true,
                                     consumer: consumer);
            }

            return Ok(message);
        }
    }
}
