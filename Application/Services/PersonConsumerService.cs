using Application.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class PersonConsumerService(ILogger<PersonConsumerService> logger) : IPersonConsumerService
{
	public Task InitConsumer(string topicName, Message<Ignore, string>? kafkaMessage)
	{
		if (kafkaMessage is not null)
		{
			var messageBody = kafkaMessage.Value;

			logger.LogInformation("----- Consumed message: {messageBody} -----", messageBody);
		}

		return Task.CompletedTask;
	}
}