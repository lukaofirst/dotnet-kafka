using Application.Interfaces;
using Infra.Kafka.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.HostedServices;

public class PersonHostedService(ILogger<PersonHostedService> logger,
	IServiceScopeFactory serviceScopeFactory,
	IKafkaConsumer kafkaConsumer) : BackgroundService
{
	private readonly string _topicName = "Person-Topic";

	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		logger.LogInformation("Starting {service}", nameof(PersonHostedService));

		var task = Task.Run(() => ProcessTopicAsync(stoppingToken), stoppingToken);

		return task;
	}

	private async Task ProcessTopicAsync(CancellationToken stoppingToken)
	{
		using var consumer = await kafkaConsumer.CreateConsumer(_topicName)!;
		consumer.Subscribe(_topicName);

		try
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				var consumerResult = consumer.Consume(stoppingToken);

				using var scope = serviceScopeFactory.CreateScope();
				var personConsumerService = scope.ServiceProvider.GetRequiredService<IPersonConsumerService>();

				await personConsumerService.InitConsumer(_topicName, consumerResult.Message);
			}
		}
		catch (Exception ex)
		{
			logger.LogError("Could not execute {service}: {ex}", nameof(PersonHostedService), ex);

			consumer.Close();
			consumer.Dispose();
		}
	}
}