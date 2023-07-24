using Confluent.Kafka;

namespace Application.Interfaces
{
	public interface IPersonConsumerService
	{
		Task InitConsumer(string topicName, Message<Ignore, string>? kafkaMessage);
	}
}