namespace Core.Entities;

public sealed record Person(
	Guid Id,
	string Name,
	int Age
);