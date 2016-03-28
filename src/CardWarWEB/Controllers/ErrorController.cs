using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CardWarWEB.Controllers
{
    [Route("Error")]
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View("e404");
        }

        [Route("404",Name = "404")]
        public IActionResult e404()
        {
            return View();
        }

        [Route("500",Name = "500")]
        public IActionResult e500()
        {
            return View();
        }
    }
}
