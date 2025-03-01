using Confluent.Kafka;

namespace Infra.Kafka.Interfaces;

public interface IKafkaConsumer
{
	Task<IConsumer<Ignore, string>>? CreateConsumer(string topicName);
}