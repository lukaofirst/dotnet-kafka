using System.Text.Json;
using Core.Entities;
using Core.Interfaces;
using Infra.Kafka.Interfaces;

namespace Application.Services
{
	public class PersonEventService : IPersonEventService
	{
		private readonly IKafkaProducer _kafkaProducer;

		public PersonEventService(IKafkaProducer kafkaProducer)
		{
			_kafkaProducer = kafkaProducer;
		}

		public async Task ProduceEvent(string topicName, Person person)
		{
			var messageBody = JsonSerializer.Serialize(person);

			await _kafkaProducer.ProduceEvent(topicName, messageBody);
		}
	}
}