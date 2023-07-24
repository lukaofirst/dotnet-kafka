using Application.Interfaces;
using Infra.Kafka.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.HostedServices
{
	public class PersonHostedService : BackgroundService
	{
		private readonly ILogger<PersonHostedService> _logger;
		private readonly IServiceScopeFactory _serviceScopeFactory;
		private readonly IKafkaConsumer _kafkaConsumer;

		private readonly string _topicName = "Person-Topic";

		public PersonHostedService(ILogger<PersonHostedService> logger,
			IServiceScopeFactory serviceScopeFactory,
			IKafkaConsumer kafkaConsumer)
		{
			_logger = logger;
			_serviceScopeFactory = serviceScopeFactory;
			_kafkaConsumer = kafkaConsumer;
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("Starting {service}", nameof(PersonHostedService));

			var task = Task.Run(() => ProcessTopicAsync(stoppingToken), stoppingToken);

			return task;
		}

		private async Task ProcessTopicAsync(CancellationToken stoppingToken)
		{
			using var consumer = await _kafkaConsumer.CreateConsumer(_topicName)!;
			consumer.Subscribe(_topicName);

			try
			{
				while (!stoppingToken.IsCancellationRequested)
				{
					var consumerResult = consumer.Consume(stoppingToken);

					using var scope = _serviceScopeFactory.CreateScope();
					var personConsumerService = scope.ServiceProvider.GetRequiredService<IPersonConsumerService>();

					personConsumerService.InitConsumer(_topicName, consumerResult.Message);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("Could not execute {service}: {ex}", nameof(PersonHostedService), ex);

				consumer.Close();
				consumer.Dispose();
			}
		}
	}
}