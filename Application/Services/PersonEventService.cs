using System.Text.Json;
using Core.Entities;
using Core.Interfaces;
using Infra.Kafka.Interfaces;

namespace Application.Services;

public class PersonEventService(IKafkaProducer kafkaProducer) : IPersonEventService
{
	public async Task ProduceEvent(string topicName, Person person)
	{
		var messageBody = JsonSerializer.Serialize(person);

		await kafkaProducer.ProduceEvent(topicName, messageBody);
	}
}