using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using DevicesManager.Core.Repositories;
using DevicesManager.Core.Models.Requests;
using DevicesManager.Web;

namespace DevicesManagerWeb.Services
{
    public class DeviceQueueService : BackgroundService
    {
        private readonly QueueSettings _queueSettings;

        private readonly IDeviceRepository _deviceRepository;
        private readonly IServiceProvider _sp;
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public DeviceQueueService(IDeviceRepository deviceRepository, IServiceProvider sp, QueueSettings queueSettings)
        {

            try
            {
                _deviceRepository = deviceRepository;
                _sp = sp;
                _queueSettings = queueSettings;

                _factory = new ConnectionFactory()
                {
                    HostName = _queueSettings.Host,
                    Port = _queueSettings.Port
                };

                if (!string.IsNullOrEmpty(_queueSettings.Username)) _factory.UserName = _queueSettings.Username;
                if (!string.IsNullOrEmpty(_queueSettings.Username)) _factory.Password = _queueSettings.Password;

                _connection = _factory.CreateConnection();

                _channel = _connection.CreateModel();

                _channel.QueueDeclare(
                    queue: _queueSettings.QueueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
            }
            catch(Exception e)
            {
                Console.WriteLine("Error connecting to queue server, please check the settings.");
            }

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_connection == null)
                return Task.CompletedTask;

            if (stoppingToken.IsCancellationRequested)
            {
                _channel.Dispose();
                _connection.Dispose();
                return Task.CompletedTask;
            }

            var consumer = new EventingBasicConsumer(_channel);


            consumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var job = JsonSerializer.Deserialize<QueueJobRequest>(Encoding.UTF8.GetString(body));

                    switch (job.Job)
                    {
                        case "create":
                            _deviceRepository.Add(JsonSerializer.Deserialize<DeviceRequest>(job.Request));
                            break;

                        case "update":
                            _deviceRepository.Update(JsonSerializer.Deserialize<DeviceRequest>(job.Request));
                            break;
                        case "delete":
                            _deviceRepository.Delete(job.Request);
                            break;

                        default:
                            Console.WriteLine($"Unknow Job {job}, received in queue {_queueSettings.QueueName}");
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"An unexpected error cocurs in queue {_queueSettings.QueueName}: {e.Message}");
                }
            };

            _channel.BasicConsume(queue: _queueSettings.QueueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }
    }
}