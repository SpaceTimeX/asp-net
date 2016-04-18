using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;
using Microsoft.Security.Application;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CardWarWEB.Controllers
{
    [Route("Interesting")]
    public class InterestingController : Controller
    {

        private bool CheckLogin2()
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
                            return true;
                        else
                            return false;
                    }
                    else
                        return false;
                }
            }
        }

        private bool CheckLogin()
        {

            if (string.IsNullOrEmpty(Request.Cookies["gusername"]) || string.IsNullOrEmpty(Request.Cookies["gpassword"]))
                return false;
            else {
                string username = Request.Cookies["gusername"];
                string password = Request.Cookies["gpassword"];
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

        [Route("CheckLogin")]
        [HttpPost]
        public object CheckLogin(string username, string password)
        {
            string Error = "未知错误";
            string state = "Error";
            if (string.IsNullOrEmpty(username))
                username = "";
            if (string.IsNullOrEmpty(password))
                password = "";
            if (username == "" || password == "")
            {
                Error = "登录失败！请填写正确的用户名和密码！";
            }
            else {
                if (Conf.Users.ContainsKey(username))
                {
                    UserConfig conf = Conf.Users[username];
                    if (conf.Password.ToUpper() == Server.MD5Encrypt(password).ToUpper())
                    {
                        List<string> flags = conf.Flags;
                        if (flags.Contains("login"))
                        {
                            if (flags.Contains("game"))
                            {
                                state = "Success";
                                Response.Cookies.Append("gusername", username);
                                Response.Cookies.Append("gpassword", Server.MD5Encrypt(password));
                            }
                            else {
                                Error = "这个账号没有游戏权限！请联系管理员！";
                            }
                        }
                        else {
                            Error = "这个账号没有登录权限！请联系管理员！";
                        }
                    }
                    else {
                        Error = "登录失败！用户名或密码错误！";
                    }

                }
                else {
                    Error = "登录失败！用户名或密码错误！";
                }
            }
            Dictionary<string, string> ss = new Dictionary<string, string>()
            {
                { "state" , state},
                { "error" , Error }
            };

            string s = JsonConvert.SerializeObject(ss);
            return s;
        }

        [Route("MainGame")]
        public IActionResult MainGame()
        {
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "gusername");
                Server.RemoveCookie(this, "gpassword");
                if (CheckLogin2())
                    return View("QuickLogin");
                return View("LoginGame");
            }

            string username = Request.Cookies["gusername"];
            if (!Conf.Users[username].IsGameActivited)
                return View("CreatePlayer");


            ViewData.Add("Nick", Conf.Users[username].iG.Nick);
            ViewData.Add("Class", Conf.Users[username].iG.Class.ToString());
            return View();
        }

        public IActionResult Index()
        {
            return Redirect("/Interesting/MainGame");
        }

        [Route("LoginGame", Name = "LoginGame")]
        public IActionResult LoginGame()
        {
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "gusername");
                Server.RemoveCookie(this, "gpassword");
                if (CheckLogin2())
                    return View("QuickLogin");
                return View();
            }


            return Redirect("/Interesting/MainGame");
        }

        [Route("CreatePlayer", Name = "CreatePlayer")]
        public IActionResult CreatePlayer()
        {
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "gusername");
                Server.RemoveCookie(this, "gpassword");
                if (CheckLogin2())
                    return View("QuickLogin");
                return View("LoginGame");
            }
            return View();
        }

        [Route("Create")]
        [HttpPost]
        public object Create(string nick, string type)
        {
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "gusername");
                Server.RemoveCookie(this, "gpassword");
                return "请登录！";
            }

            if (string.IsNullOrEmpty(nick) || string.IsNullOrEmpty(type))
                return "请输入完整！并选择职业！";
            if (nick.Length > 16)
            {
                return "创建失败！名称过长！>16";
            }
            string username = Request.Cookies["gusername"];
            if (Conf.Users[username].IsGameActivited)
                return "不能再创建人物了！";

            int t = int.Parse(type);
            if (t < 0 || t > 3)
                return "职业选择错误！";

            iClasses icl = (iClasses)t;
            iPro iP = new iPro();


            if (icl == iClasses.战士)
            {
                iP.HP = 80;
                iP.MP = 45;
                iP.AD = 11;
                iP.AP = 0;
                iP.AR = 11;
            }
            else if (icl == iClasses.法师)
            {
                iP.HP = 70;
                iP.MP = 60;
                iP.AD = 8;
                iP.AP = 6;
                iP.AR = 7;
            }
            else if (icl == iClasses.刺客)
            {
                iP.HP = 65;
                iP.MP = 40;
                iP.AD = 16;
                iP.AP = 0;
                iP.AR = 7;
            }
            else if (icl == iClasses.骑士)
            {
                iP.HP = 90;
                iP.MP = 40;
                iP.AD = 6;
                iP.AP = 6;
                iP.AR = 13;
            }
            ObjectCopy oc = new ObjectCopy();

            iPro OriginalPro = (iPro)oc.DeepCopy(iP);
            iGame Game = new iGame();
            Game.Nick = nick;
            Game.Username = username;
            Game.Pro = iP;
            Game.OriginalPro = OriginalPro;
            Game.Class = icl;
            Conf.Users[username].IsGameActivited = true;
            Conf.Users[username].iG = Game;
            return "Success";
        }

        [Route("GetiPro")]
        public object GetiPro()
        {
            string state = "Error";
            string Error = "未知错误";
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "gusername");
                Server.RemoveCookie(this, "gpassword");
                Error = "请登录！";
                goto end;
            }

            string username = Request.Cookies["gusername"];
            if (!Conf.Users[username].IsGameActivited)
            {
                Error = "请创建人物！";
                goto end;
            }
            List<object> l = new List<object>();
            l.Add(Conf.Users[username].iG);
            long now = Conf.Users[username].iG.Exp;
            long least = iConf.GetLeastExps(now);
            long all = now + least;
            Dictionary<string, object> d = new Dictionary<string, object>();
            d.Add("now", now);
            d.Add("least", least);
            d.Add("all", all);
            l.Add(d);
            string t = JsonConvert.SerializeObject(l);
            return t;
            end:
            Dictionary<string, object> ss = new Dictionary<string, object>() {
                { "state",state},
                { "error",Error}
            };

            string s = JsonConvert.SerializeObject(ss);
            return s;
        }

        [Route("SavePoint")]
        [HttpPost]
        public object SavePoint(string a, string b, string c, string d, string e)
        {

            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "gusername");
                Server.RemoveCookie(this, "gpassword");
                return "请登录！";
            }

            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b) || string.IsNullOrEmpty(c) || string.IsNullOrEmpty(d) || string.IsNullOrEmpty(e))
                return "错误的参数";

            string username = Request.Cookies["gusername"];
            iGame ig = Conf.Users[username].iG;
            long aa, bb, cc, dd, ee;
            aa = long.Parse(a);
            bb = long.Parse(b);
            cc = long.Parse(c);
            dd = long.Parse(d);
            ee = long.Parse(e);

            long aaa, bbb, ccc, ddd, eee;
            aaa = ig.Pro.LL;
            bbb = ig.Pro.ZL;
            ccc = ig.Pro.NL;
            ddd = ig.Pro.SB;
            eee = ig.Pro.JZ;

            long last = ig.Pro.NonUsedPoint;
            long used = aa - aaa + bb - bbb + cc - ccc + dd - ddd + ee - eee;
            if (used > last)
                return "错误的分配";

            ig.Pro.NonUsedPoint -= used;
            ig.Pro.UsedPoint += used;
            ig.Pro.LL = aa;
            ig.Pro.ZL = bb;
            ig.Pro.NL = cc;
            ig.Pro.SB = dd;
            ig.Pro.JZ = ee;

            return "Success";
        }
    }
}
