using Confluent.Kafka;
using Infra.Kafka.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infra.Kafka.Services;

public class KafkaProducer(ILogger<BaseKafka> baseLogger,
	ILogger<KafkaProducer> logger, IConfiguration configuration)
	: BaseKafka(baseLogger, configuration), IKafkaProducer
{
	private readonly ILogger _logger = logger;

	public async Task ProduceEvent(string topicName, string messageBody)
	{
		try
		{
			var producer = GetProducer();
			await EnsureTopic(topicName);

			var message = new Message<Null, string>
			{
				Value = messageBody
			};

			await producer.ProduceAsync(topicName, message);
		}
		catch (Exception ex)
		{
			_logger.LogError("Could not producer event - {ex}", ex);
		}
	}
}