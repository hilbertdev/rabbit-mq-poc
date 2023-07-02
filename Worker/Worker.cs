using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Worker
{
    class Worker
    {
        public static void Main()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            // using statement ensures that the channel is closed and disposed
            // when it leaves the scope of the using statement.
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // Declaring a queue is idempotent - it will only be created if it doesn't exist already.
                channel.QueueDeclare(queue: "task_queue",
                         durable: true,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                Console.WriteLine(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($" [x] Received {message}");

                    int dots = message.Split('.').Length - 1;
                    Thread.Sleep(dots * 1000);

                    Console.WriteLine(" [x] Done");
                };
                channel.BasicConsume(queue: "task_queue",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}