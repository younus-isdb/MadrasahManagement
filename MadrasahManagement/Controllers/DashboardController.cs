using MadrasahManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadrasahManagement.Controllers
{

[Authorize]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        if (User.IsInRole("Admin"))
            return RedirectToAction("AdminDashboard");

        if (User.IsInRole("Teacher"))
            return RedirectToAction("TeacherDashboard");

        if (User.IsInRole("Student"))
            return RedirectToAction("StudentDashboard");

        return Content("Unauthorized");
    }

    [Authorize(Roles = "Admin")]
    public IActionResult AdminDashboard() => View();

    [Authorize(Roles = "Teacher")]
    public IActionResult TeacherDashboard() => View();

    [Authorize(Roles = "Student")]
    public IActionResult StudentDashboard() => View();
}
}
