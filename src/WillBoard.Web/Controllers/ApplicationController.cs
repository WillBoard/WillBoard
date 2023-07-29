using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Enums;
using WillBoard.Web.Filters;

namespace WillBoard.Web.Controllers
{
    [OnlineCounterFilter]
    public class ApplicationController : Controller
    {
        public ApplicationController()
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new ApplicationViewModel()
            {
                Title = "Overview",
                ViewType = ViewType.Application
            };

            return View("Index", model);
        }
    }
}