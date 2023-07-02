using System.Text;
using RabbitMQ.Client;

namespace NewTask
{
    class NewTask
    {
        public static void Main(string[] args)
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

                string message = GetMessage(args);
                // The message content is a byte array, so you can encode whatever you like there.
                var body = Encoding.UTF8.GetBytes(message);
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                // The basicPublish method is used to publish a message.
                // It has a number of parameters, but the most important ones are
                // the exchange, the routing key, and the message body.
                // The exchange parameter allows us to specify the exchange to which
                // the message should be sent.
                // The empty string denotes the default or nameless exchange: messages
                // are routed to the queue with the name specified by routingKey, if it exists.
                // The routingKey parameter is the message queue to which the message should be sent.
                // The queue parameter is the message body.
                channel.BasicPublish(exchange: "",
                                     routingKey: "task_queue",
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
        static string GetMessage(string[] args)
        {
            return ((args.Length > 0)
                   ? string.Join(" ", args)
                   : "Hello World!");
        }
    }
}