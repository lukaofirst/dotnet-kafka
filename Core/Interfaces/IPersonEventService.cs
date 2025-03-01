using Core.Entities;

namespace Core.Interfaces;

public interface IPersonEventService
{
	Task ProduceEvent(string topicName, Person person);
}