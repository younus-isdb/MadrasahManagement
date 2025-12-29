using MadrasahManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.ApiControllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TeacherApiController : ControllerBase
	{
		private readonly MadrasahDbContext _context;
		public TeacherApiController(MadrasahDbContext context)
		{
			_context = context;
		}
		[HttpGet]
		public IActionResult GetAll()
		{
			
			return Ok();
		}
	}
}
