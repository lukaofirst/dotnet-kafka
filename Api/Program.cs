using Application.HostedServices;
using Application.Interfaces;
using Application.Services;
using Core.Interfaces;
using Infra.Kafka.Interfaces;
using Infra.Kafka.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IPersonEventService, PersonEventService>();
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();
builder.Services.AddSingleton<IKafkaConsumer, KafkaConsumer>();

builder.Services.AddScoped<IPersonConsumerService, PersonConsumerService>();
builder.Services.AddHostedService<PersonHostedService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
