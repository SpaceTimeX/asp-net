using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CardWarWEB.Controllers
{
    [Route("Interesting")]
    public class InterestingController : Controller
    {
        private bool CheckLogin()
        {

            if (string.IsNullOrEmpty(Request.Cookies["username"]) || string.IsNullOrEmpty(Request.Cookies["password"]))
                return false;
            else {
                string username = Request.Cookies["username"];
                string password = Request.Cookies["password"];
                if (!Conf.Users.ContainsKey(username))
                    return false;
                else {
                    string md5_password = Conf.Users[username].Password;
                    if (password.ToUpper() == md5_password.ToUpper())
                    {
                        List<string> flags = Conf.Users[username].Flags;
                        if (flags.Contains("login"))
                        {
                            if (flags.Contains("game"))
                                return true;
                            else
                                return false;
                        }
                        else
                            return false;
                    }
                    else
                        return false;
                }
            }
        }

        public IActionResult Index()
        {
            return View();
        }


        [Route("CreatePlayer")]
        public IActionResult CreatePlayer() {
            return View();
        }
    }
}
