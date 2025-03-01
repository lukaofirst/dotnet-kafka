using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infra.Kafka.Services;

public abstract class BaseKafka
{
	private readonly ILogger _logger;
	private readonly IConfiguration _configuration;

	private readonly string _connStr;
	private readonly string _groupId;
	private readonly int _retentionInMs;
	private readonly int _numPartitions;

	public BaseKafka(ILogger logger, IConfiguration configuration)
	{
		_logger = logger;
		_configuration = configuration;

		_connStr = _configuration["Kafka:Url"]!;
		_groupId = _configuration["Kafka:GroupId"]!;
		_retentionInMs = int.Parse(_configuration["Kafka:RetentionInMs"]!);
		_numPartitions = int.Parse(_configuration["Kafka:NumPartitions"]!);
	}

	private ProducerConfig GetProducerConfig => new()
	{
		BootstrapServers = _connStr,
		Acks = Acks.Leader,
	};

	private protected ConsumerConfig GetConsumerConfig => new()
	{
		BootstrapServers = _connStr,
		GroupId = _groupId,
		AutoOffsetReset = AutoOffsetReset.Earliest,
		AutoCommitIntervalMs = 1000
	};

	private AdminClientConfig GetAdminClientConfig => new()
	{
		BootstrapServers = _connStr
	};

	private protected IProducer<Null, string> GetProducer()
	{
		var producer = new ProducerBuilder<Null, string>(GetProducerConfig).Build();

		return producer;
	}

	private protected async Task EnsureTopic(string topicName)
	{
		try
		{
			var adminClient = new AdminClientBuilder(GetAdminClientConfig).Build();
			var topics = adminClient.GetMetadata(TimeSpan.FromSeconds(1)).Topics;

			var topicExist = topics.Any(x => x.Topic.Equals(topicName));

			if (!topicExist)
			{
				var retentionInMilliseconds = _retentionInMs;

				var topicSpec = new TopicSpecification
				{
					Name = topicName,
					NumPartitions = _numPartitions,
					ReplicationFactor = 1,
					Configs = new()
					{
						{ "retention.ms", retentionInMilliseconds.ToString() }
					}
				};

				await adminClient.CreateTopicsAsync(new[] { topicSpec });
			}
		}
		catch (CreateTopicsException ex)
		{
			_logger.LogError("Topic already exist - Details: {ex}", ex);
		}
	}
}