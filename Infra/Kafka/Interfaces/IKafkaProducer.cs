namespace Infra.Kafka.Interfaces
{
	public interface IKafkaProducer
	{
		Task ProduceEvent(string topicName, string messageBody);
	}
}