using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infra.Kafka.Services;

public abstract class BaseKafka
{
	private protected IProducer<Null, string> GetProducer { get; }
	private protected IConsumer<Ignore, string> GetConsumer { get; }

	private readonly ILogger _logger;
	private readonly IConfiguration _configuration;

	private readonly string _connStr;
	private readonly string _groupId;
	private readonly int _retentionInMs;
	private readonly int _numPartitions;

	private readonly IAdminClient _adminClient;
	private readonly List<string> _topics;

	public BaseKafka(ILogger logger, IConfiguration configuration)
	{
		_logger = logger;
		_configuration = configuration;

		_connStr = _configuration["Kafka:Url"]!;
		_groupId = _configuration["Kafka:GroupId"]!;
		_retentionInMs = int.Parse(_configuration["Kafka:RetentionInMs"]!);
		_numPartitions = int.Parse(_configuration["Kafka:NumPartitions"]!);

		_adminClient = new AdminClientBuilder(AdminClientConfig).Build();

		GetProducer = new ProducerBuilder<Null, string>(ProducerConfig).Build();
		GetConsumer = new ConsumerBuilder<Ignore, string>(ConsumerConfig).Build();

		_topics = [.. _adminClient.GetMetadata(TimeSpan.FromSeconds(1)).Topics.Select(x => x.Topic)];
	}

	private AdminClientConfig AdminClientConfig => new()
	{
		BootstrapServers = _connStr
	};

	private ProducerConfig ProducerConfig => new()
	{
		BootstrapServers = _connStr,
		Acks = Acks.Leader,
	};

	private ConsumerConfig ConsumerConfig => new()
	{
		BootstrapServers = _connStr,
		GroupId = _groupId,
		AutoOffsetReset = AutoOffsetReset.Earliest,
		AutoCommitIntervalMs = 1000
	};

	private protected async Task EnsureTopic(string topicName)
	{
		try
		{
			var topicExist = _topics.Any(x => x.Equals(topicName));

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

				await _adminClient.CreateTopicsAsync([topicSpec]);
				_topics.Add(topicName);
			}
		}
		catch (CreateTopicsException ex)
		{
			_logger.LogError("Topic already exist - Details: {ex}", ex);
		}
	}
}