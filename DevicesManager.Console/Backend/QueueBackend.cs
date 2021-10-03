using System;
using System.Text.Json;
using System.Threading.Tasks;
using DevicesManager.Core.Models.Requests;
using DevicesManager.Console.Models;
using RabbitMQ.Client;

namespace DevicesManager.Console.Backend
{
    public class QueueBackend : APIBackend, IBackend
    {
        private readonly BackendSettings _settings;
        private readonly IConnection _connection;

        public QueueBackend(BackendSettings settings) : base(settings.HostAPI)
        {
            _settings = settings;

            var factory = new ConnectionFactory()
            {
                HostName = _settings.Queue.Host,
                Port = _settings.Queue.Port
            };

            if (!string.IsNullOrEmpty(_settings.Queue.Username)) factory.UserName = _settings.Queue.Username;
            if (!string.IsNullOrEmpty(_settings.Queue.Username)) factory.Password = _settings.Queue.Password;

            try
            {
                _connection = factory.CreateConnection();
            }
            catch (Exception e)
            {
                System.Console.WriteLine($"An error occur connection to Queue Server, API will be used: {e.Message}");
            }
        }

        public override async Task<bool> Add(DeviceRequest deviceRequest)
        {
            if (_connection == null) return await base.Add(deviceRequest);

            return await sendJob(new QueueJobRequest()
            {
                Job = "create",
                Request = JsonSerializer.Serialize(deviceRequest)
            });
        }

        public override async Task<bool> Update(DeviceRequest deviceRequest)
        {
            if (_connection == null) return await base.Update(deviceRequest);

            return await sendJob(new QueueJobRequest()
            {
                Job = "update",
                Request = JsonSerializer.Serialize(deviceRequest)
            });
        }

        public override async Task<bool> Delete(string id)
        {
            if (_connection == null) return await base.Delete(id);

            return await sendJob(new QueueJobRequest()
            {
                Job = "delete",
                Request = id
            });            
        }

        private async Task<bool> sendJob(QueueJobRequest job)
        {
            try
            {
                using (var channel = _connection.CreateModel())
                {
                    channel.QueueDeclare(queue: _settings.Queue.QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                    var json = JsonSerializer.Serialize(job);
                    var body = System.Text.Encoding.UTF8.GetBytes(json);

                    channel.BasicPublish(exchange: "", routingKey: _settings.Queue.QueueName, basicProperties: null, body: body);
                }

                return await Task.FromResult(true);
            }
            catch (Exception e)
            {
                System.Console.WriteLine($"An error occurs sending job to the queue: {e.Message}");
            }

            return await Task.FromResult(false);            
        }
    }
}