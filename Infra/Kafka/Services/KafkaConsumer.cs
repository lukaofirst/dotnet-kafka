using Confluent.Kafka;
using Infra.Kafka.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infra.Kafka.Services
{
	public class KafkaConsumer : BaseKafka, IKafkaConsumer
	{
		private readonly ILogger _logger;

		public KafkaConsumer(ILogger<BaseKafka> baseLogger,
			ILogger<KafkaConsumer> logger,
			IConfiguration configuration)
			: base(baseLogger, configuration)
		{
			_logger = logger;
		}

		public async Task<IConsumer<Ignore, string>>? CreateConsumer(string topicName)
		{
			try
			{
				var consumer = new ConsumerBuilder<Ignore, string>(GetConsumerConfig).Build();
				await EnsureTopic(topicName);

				return consumer;
			}
			catch (Exception ex)
			{
				_logger.LogError("Could not create consumer - {ex}", ex);

				return null!;
			}
		}
	}
}