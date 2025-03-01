using System.Text;
using Confluent.Kafka;

namespace Infra.Kafka.Extensions;

public static class KafkaExtensions
{
	public static string GetValueFromKafkaHeader(this Message<Ignore, string> kafkaMessage, string headerFieldName)
	{
		var header = kafkaMessage.Headers
			.First(h => h.Key.Equals(headerFieldName, StringComparison.OrdinalIgnoreCase));

		var headerBytes = header.GetValueBytes();

		var headerValue = Encoding.UTF8.GetString(headerBytes);

		return headerValue;
	}

	public static Headers SetKafkaHeadersFromEntity(this object entity)
	{
		var kafkaHeaders = new Headers();

		var type = entity.GetType();
		var properties = type.GetProperties();

		foreach (var property in properties)
			kafkaHeaders.Add(property.Name, Encoding.UTF8.GetBytes(property.GetValue(entity)!.ToString()!));

		return kafkaHeaders;
	}

	public static bool IsValidValue(this Message<Ignore, string> kafkaMessage)
	{
		return kafkaMessage.Value is not null;
	}
}
