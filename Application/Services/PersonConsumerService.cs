using Application.HostedServices;
using Application.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
	public class PersonConsumerService : IPersonConsumerService
	{
		private readonly ILogger<PersonConsumerService> _logger;

		public PersonConsumerService(ILogger<PersonConsumerService> logger)
		{
			_logger = logger;
		}

		public Task InitConsumer(string topicName, Message<Ignore, string>? kafkaMessage)
		{
			if (kafkaMessage is not null)
			{
				var messageBody = kafkaMessage.Value;

				Console.WriteLine($"----- Consumed message: {messageBody} -----");
			}

			return Task.CompletedTask;
		}
	}
}