using System.ComponentModel.DataAnnotations;

namespace RabbitMq.Console.Publisher.Configuration;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMQ";

    [Required]
    public string HostName { get; set; } = string.Empty;

    [Range(1, 65535)]
    public int Port { get; set; } = 5672;

    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string VirtualHost { get; set; } = "/";

    [Required]
    public string QueueName { get; set; } = string.Empty;
}1