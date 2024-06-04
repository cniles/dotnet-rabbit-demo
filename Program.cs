using System;
using Steeltoe.Connector.RabbitMQ;
using Steeltoe.Messaging.RabbitMQ.Config;
using Steeltoe.Messaging.RabbitMQ.Connection;
using Steeltoe.Messaging.RabbitMQ.Core;
using Steeltoe.Messaging.RabbitMQ.Extensions;

using Steeltoe.Extensions.Configuration.CloudFoundry;

var builder = WebApplication.CreateBuilder(args);

builder.AddCloudFoundryConfiguration();
builder.Configuration.AddCloudFoundry().Build();

var rabbitSection = builder.Configuration.GetSection(RabbitOptions.PREFIX);

builder.Services.Configure<RabbitOptions>(rabbitSection);

builder.Services.AddRabbitMQConnection(builder.Configuration);
builder.Services.AddRabbitServices();
builder.Services.AddRabbitAdmin();
builder.Services.AddRabbitTemplate();
builder.Services.AddRabbitQueue(new Queue("myqueue"));

var app = builder.Build();

app.MapGet("/", () => "Hello world");
app.MapPost("/message", CreateMessage);

app.Run();

static async Task<string> CreateMessage(Message message, RabbitTemplate template) {
	template.ConvertAndSend("myqueue", message.Value);
	var val = template.ReceiveAndConvert<string>("myqueue");
	Console.WriteLine("Received message {0}", val);
	return "OK";
}

class Message{
	public String Value { get; set;} = String.Empty;
}