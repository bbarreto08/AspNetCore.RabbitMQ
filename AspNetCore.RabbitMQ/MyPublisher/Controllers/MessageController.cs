using Microsoft.AspNetCore.Mvc;
using MyPublisher.Model;
using Newtonsoft.Json;
using RabbitMQ.Client;
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
            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "barreto", Password = "barretoPass" };

            using(var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                string message = JsonConvert.SerializeObject(msg);

                var body = Encoding.UTF8.GetBytes(message);

                channel.QueueDeclare(queue: "fila01",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                channel.BasicPublish(exchange: "",
                                     routingKey: "fila01",
                                     basicProperties: null,
                                     body: body);
            }

            return Ok();
        }       
    }
}
