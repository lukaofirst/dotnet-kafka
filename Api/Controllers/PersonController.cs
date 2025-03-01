using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonController(IPersonEventService personEventService) : ControllerBase
{
	private readonly string _topicName = "Person-Topic";

	[HttpPost]
	public async Task<IActionResult> Post(Person person)
	{
		await personEventService.ProduceEvent(_topicName, person);

		return NoContent();
	}
}