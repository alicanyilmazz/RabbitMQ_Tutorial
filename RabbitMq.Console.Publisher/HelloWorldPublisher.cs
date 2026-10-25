using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMq.Console.Publisher.Configuration;
using RabbitMQ.Client;
using System.Text;

namespace RabbitMq.Console.Publisher;

public sealed class HelloWorldPublisher
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<HelloWorldPublisher> _logger;

    public HelloWorldPublisher(IOptions<RabbitMqOptions> options, ILogger<HelloWorldPublisher> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task PublishAsync(CancellationToken cancellationToken = default)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost,
            ClientProvidedName = "RabbitMQ.Publisher"
        };

        await using var connection = await factory.CreateConnectionAsync(cancellationToken);

        var channelOptions = new CreateChannelOptions(publisherConfirmationsEnabled: true, publisherConfirmationTrackingEnabled: true);

        await using var channel = await connection.CreateChannelAsync(channelOptions, cancellationToken);

        await channel.QueueDeclareAsync(queue: _options.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: cancellationToken);

        const string message = "Hello RabbitMQ!";

        var body = Encoding.UTF8.GetBytes(message);

        var properties = new BasicProperties { Persistent = true, ContentType = "text/plain", MessageId = Guid.NewGuid().ToString() };

        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: _options.QueueName, mandatory: true, basicProperties: properties, body: body, cancellationToken: cancellationToken);

        _logger.LogInformation("Message published to queue {QueueName}: {Message}", _options.QueueName, message);
    }
}