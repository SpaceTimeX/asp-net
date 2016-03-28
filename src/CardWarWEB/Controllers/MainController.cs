using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CardWarWEB.Controllers
{
    [Route("Main")]
    public class MainController : Controller
    {

        public IActionResult Index()
        {
            return View("Login");
        }

        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }


        [Route("Login", Name = "Login")]
        public IActionResult Login()
        {
            return View();
        }
    }
}
