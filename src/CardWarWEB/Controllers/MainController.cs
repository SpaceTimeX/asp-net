using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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


        [Route("checkLogin")]
        public object checkLogin()
        {
            string username = Request.Form["username"];
            string password = Request.Form["password"];

            string Error = "未知错误";
            string state = "Error";
            Dictionary<string, string> ss = new Dictionary<string, string>()
            {
                { "state" , state},
                { "error" , Error }
            };

            string s = JsonConvert.SerializeObject(ss);
            return s;
        }
    }
}
