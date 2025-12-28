using Microsoft.AspNetCore.Mvc;

namespace MadrasahManagement.Controllers
{
    public class StudentDashBoard : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
