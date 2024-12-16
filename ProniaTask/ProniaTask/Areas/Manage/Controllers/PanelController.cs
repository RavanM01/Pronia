using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProniaTask.Areas.Manage.Controllers
{
    public class PanelController : Controller
    {
        [Area("Manage")]
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
