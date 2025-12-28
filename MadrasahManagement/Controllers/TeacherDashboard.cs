using Microsoft.AspNetCore.Mvc;

namespace MadrasahManagement.Controllers
{
    public class TeacherDashboard : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
