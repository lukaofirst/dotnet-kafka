namespace Core.Entities
{
	public sealed record Person(
		Guid id,
		string name,
		int age
	);
}