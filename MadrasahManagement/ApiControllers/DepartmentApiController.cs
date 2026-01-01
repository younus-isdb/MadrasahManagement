using MadrasahManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MadrasahManagement.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentApiController : ControllerBase
    {
        private readonly MadrasahDbContext _db;
       

        public DepartmentApiController(MadrasahDbContext db)
        {
            _db = db;
        }
    }
}
