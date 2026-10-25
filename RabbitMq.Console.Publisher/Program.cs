using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMq.Console.Publisher;
using RabbitMq.Console.Publisher.Configuration;

var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings { Args = args, ContentRootPath = AppContext.BaseDirectory });

builder.Services.AddOptions<RabbitMqOptions>().Bind(builder.Configuration.GetSection(RabbitMqOptions.SectionName)).ValidateDataAnnotations().ValidateOnStart();

builder.Services.AddTransient<HelloWorldPublisher>();

using var host = builder.Build();

await host.StartAsync();

try
{
    var publisher = host.Services.GetRequiredService<HelloWorldPublisher>();

    await publisher.PublishAsync();
}
finally
{
    await host.StopAsync();
}