using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PersonController : ControllerBase
	{
		private readonly IPersonEventService _personEventService;

		private readonly string _topicName = "Person-Topic";

		public PersonController(IPersonEventService personEventService)
		{
			_personEventService = personEventService;
		}

		[HttpPost]
		public async Task<IActionResult> Post(Person person)
		{
			await _personEventService.ProduceEvent(_topicName, person);

			return NoContent();
		}
	}
}