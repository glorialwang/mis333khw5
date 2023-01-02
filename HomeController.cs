using Microsoft.AspNetCore.Mvc;

//TODO: Upddate this namespace to match your project name
namespace Wang_Gloria_HW5.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View("Index");
        }
    }
}
