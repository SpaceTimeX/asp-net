using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Http.Internal;
using Microsoft.Extensions.Primitives;
using Microsoft.Security.Application;
using System.Threading;
using Microsoft.AspNet.Http;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Net.Http.Headers;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CardWarWEB.Controllers
{
    [Route("Main")]
    public class MainController : Controller
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
                            return true;
                        else
                            return false;
                    }
                    else
                        return false;
                }
            }
        }

        public object Index()
        {
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                return View("Login");
            }
            ViewBag.Username = Request.Cookies["username"];
            string username = Request.Cookies["username"];
            List<string> flags = Conf.Users[username].Flags;
            if (!flags.Contains("mainpage"))
            {
                return "这个账号没有浏览主页权限！请联系管理员！";

            }
            ViewBag.ID1ThreadsCount = Conf.AreaThreads[1].Count.ToString();
            ViewBag.ID2ThreadsCount = Conf.AreaThreads[2].Count.ToString();
            ViewBag.ID3ThreadsCount = Conf.AreaThreads[3].Count.ToString();

            ViewBag.UserMessages = Conf.Users[username].Messages.Count - Conf.Users[username].ReadCount;
            int prefixid = Conf.Users[username].PrefixID;
            if (prefixid == -1)
                ViewBag.Prefix = "无";
            else
                ViewBag.Prefix = Conf.Users[username].Prefix[prefixid];

            ViewBag.Rank = Conf.Users[username].Rank;
            ViewBag.Coins = Conf.Users[username].Coin;
            ViewBag.Prestige = Conf.Users[username].Prestige;
            ViewBag.Point = Conf.Users[username].Point;
            return View();
        }

        [Route("Register")]
        public IActionResult Register()
        {
            if (CheckLogin())
            {
                string username = Request.Cookies["username"];
                Response.Redirect("/Home");
            }

            return View();
        }

        [Route("Search")]
        public object Search()
        {
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                return View("Login");
            }
            string username = Request.Cookies["username"];
            List<string> flags = Conf.Users[username].Flags;
            if (!flags.Contains("search"))
            {
                return "这个账号没有搜索权限！请联系管理员！";

            }
            return "Building...";
        }

        [Route("Login", Name = "Login")]
        public IActionResult Login()
        {
            return View();
        }

        [Route("Logout")]
        public IActionResult Logout()
        {
            Server.RemoveCookie(this, "username");
            Server.RemoveCookie(this, "password");
            return View();
        }
        [Route("ViewThreads/{id?}")]
        public object ViewThreads(string id)
        {
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                return View("Login");
            }

            if (string.IsNullOrEmpty(id))
            {
                return "Error";
            }
            else {
                int iid = int.Parse(id);
                if (!Conf.AreaName.ContainsKey(iid))
                    return "Error";
                else
                    ViewBag.IDname = Conf.AreaName[iid];


                ViewBag.Username = Request.Cookies["username"];
                string username = Request.Cookies["username"];
                List<string> flags = (List<string>)Conf.Users[username].Flags;
                if (!flags.Contains("view"))
                {
                    return "这个账号没有浏览帖子权限！请联系管理员！";

                }
                string note = Conf.AreaNote[iid];
                ViewBag.AreaNote = note;
                ViewBag.Block = Conf.Blocks[iid];
                ViewBag.UserMessages = Conf.Users[username].Messages.Count - Conf.Users[username].ReadCount;
                int prefixid = Conf.Users[username].PrefixID;
                if (prefixid == -1)
                    ViewBag.Prefix = "无";
                else
                    ViewBag.Prefix = Conf.Users[username].Prefix[prefixid];

                ViewBag.Rank = Conf.Users[username].Rank;
                ViewBag.Coins = Conf.Users[username].Coin;
                ViewBag.Prestige = Conf.Users[username].Prestige;
                ViewBag.Point = Conf.Users[username].Point;
                ViewBag.ID = iid;
                return View();
            }
        }

        [Route("ViewThread/{id?}")]
        public object ViewThread(string id)
        {
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                return View("Login");
            }
            if (string.IsNullOrEmpty(id))
            {
                return "Error";
            }
            int iid = int.Parse(id);
            if (!Conf.Threads.ContainsKey(iid))
                return "Error";
            else {
                foreach (int i in Conf.AreaThreads.Keys)
                {
                    List<int> li = Conf.AreaThreads[i];
                    if (li.Contains(iid))
                    {
                        ViewBag.IDname = Conf.AreaName[i];
                        ViewBag.ID = i;
                        break;
                    }
                }

                if (ViewBag.IDname == null)
                    return "Error";
            }

            ViewBag.ThreadTitle = Conf.Threads[iid].Title;
            ViewBag.Block = Conf.Blocks[ViewBag.ID];
            string username = Request.Cookies["username"];
            ViewBag.Username = Request.Cookies["username"];
            List<string> flags = (List<string>)Conf.Users[username].Flags;
            if (!flags.Contains("view"))
            {
                return "这个账号没有浏览帖子权限！请联系管理员！";

            }
            ViewBag.UserMessages = Conf.Users[username].Messages.Count - Conf.Users[username].ReadCount;
            int prefixid = (int)Conf.Users[username].PrefixID;
            if (prefixid == -1)
                ViewBag.Prefix = "无";
            else
                ViewBag.Prefix = Conf.Users[username].Prefix[prefixid];

            ViewBag.Rank = Conf.Users[username].Rank;
            ViewBag.Coins = Conf.Users[username].Coin;
            ViewBag.Prestige = Conf.Users[username].Prestige;
            ViewBag.Point = Conf.Users[username].Point;
            ViewBag.ThreadID = id;
            return View();

        }
        [Route("checkLogin")]
        [HttpPost]
        public object checkLogin(string username, string password)
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
                            state = "Success";
                            Response.Cookies.Append("username", username);
                            Response.Cookies.Append("password", Server.MD5Encrypt(password));
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

        [Route("checkRegister")]
        [HttpPost]
        public object checkRegister(string username, string password, string confirmpassword, string email)
        {
            string Error = "未知错误";
            string state = "Error";
            if (string.IsNullOrEmpty(username))
                username = "";
            if (string.IsNullOrEmpty(password))
                password = "";
            if (string.IsNullOrEmpty(confirmpassword))
                confirmpassword = "";
            if (string.IsNullOrEmpty(email))
                email = "";
            username = Sanitizer.GetSafeHtmlFragment(username);
            if (username == "" || password == "" || confirmpassword == "" || email == "")
            {
                Error = "注册失败！请填写正确的信息！";
            }
            else {
                if (username.Length > 16)
                {
                    Error = "注册失败！用户名过长！>16";
                }
                else {
                    if (password != confirmpassword)
                    {
                        Error = "注册失败！两次输入的密码不同！";
                    }
                    else {
                        if (Server.IsUsername(username) && Server.IsPassword(password))
                        {
                            if (Conf.Users.ContainsKey(username))
                            {
                                Error = "注册失败！这个账号已被注册过了！";
                            }
                            else {
                                if (Conf.Emails.Contains(email))
                                {
                                    Error = "注册失败！这个邮箱已经被注册过了！";
                                }
                                else {
                                    new Thread((ThreadStart)delegate { Server.SendMail("欢迎进入 时空之城！", "欢迎您，" + username + "<br/>感谢您的注册，欢迎进入 时空之城！<br/>请手动在个人空间进行邮件验证！谢谢！", email); }).Start();
                                    UserConfig uc = new UserConfig()
                                    {
                                        Username = username,
                                        Password = Server.MD5Encrypt(password),
                                        Email = email
                                    };
                                    Conf.Users.Add(username, uc);
                                    Conf.Emails.Add(email);
                                    state = "Success";

                                }
                            }
                        }
                        else
                        {
                            Error = "注册失败！账号或者密码不合法！";
                        }


                    }
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

        [Route("Reply")]
        [HttpPost]
        public object Reply(string id)
        {

            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                return View("Login");
            }
            if (string.IsNullOrEmpty(id))
            {
                return "Error";
            }
            int iid = int.Parse(id);

            if (!Conf.Threads.ContainsKey(iid))
                return "Error";

            foreach (int i in Conf.AreaThreads.Keys)
            {
                List<int> li = Conf.AreaThreads[i];
                if (li.Contains(iid))
                {
                    ViewBag.IDname = Conf.AreaName[i];
                    ViewBag.ID = i;
                    break;
                }
            }

            if (ViewBag.IDname == null)
                return "Error";

            ViewBag.ThreadTitle = Conf.Threads[iid].Title;
            ViewBag.Block = Conf.Blocks[ViewBag.ID];
            string username = Request.Cookies["username"];
            ViewBag.Username = Request.Cookies["username"];
            List<string> flags = Conf.Users[username].Flags;
            if (!flags.Contains("apply"))
            {
                return "这个账号没有修改个人信息权限！请联系管理员！";

            }
            ViewBag.UserMessages = Conf.Users[username].Messages.Count - Conf.Users[username].ReadCount;
            int prefixid = Conf.Users[username].PrefixID;
            if (prefixid == -1)
                ViewBag.Prefix = "无";
            else
                ViewBag.Prefix = Conf.Users[username].Prefix[prefixid];

            ViewBag.Rank = Conf.Users[username].Rank;
            ViewBag.Coins = Conf.Users[username].Coin;
            ViewBag.Prestige = Conf.Users[username].Prestige;
            ViewBag.Point = Conf.Users[username].Point;
            ViewBag.ThreadID = id;

            return View();
        }


        [Route("Post")]
        [HttpPost]
        public object Post(string id)
        {
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                return View("Login");
            }
            if (string.IsNullOrEmpty(id))
            {
                return "Error";
            }
            int iid = int.Parse(id);
            if (!Conf.AreaName.ContainsKey(iid))
                return "Error";
            else
                ViewBag.IDname = Conf.AreaName[iid];

            ViewBag.ID = id;
            string username = Request.Cookies["username"];
            ViewBag.Username = Request.Cookies["username"];
            string note = Conf.AreaNote[iid];
            ViewBag.AreaNote = note;
            ViewBag.Block = Conf.Blocks[iid];
            ViewBag.UserMessages = Conf.Users[username].Messages.Count - Conf.Users[username].ReadCount;
            int prefixid = (int)Conf.Users[username].PrefixID;
            if (prefixid == -1)
                ViewBag.Prefix = "无";
            else
                ViewBag.Prefix = Conf.Users[username].Prefix[prefixid];

            ViewBag.Rank = Conf.Users[username].Rank;
            ViewBag.Coins = Conf.Users[username].Coin;
            ViewBag.Prestige = Conf.Users[username].Prestige;
            ViewBag.Point = Conf.Users[username].Point;
            ViewBag.ID = iid;
            return View();
        }

        [Route("Repo")]
        [HttpPost]
        public object Repo(string id, string content)
        {

            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                return View("Login");
            }
            string Error = "未知错误";
            string state = "Error";

            if (string.IsNullOrEmpty(id))
            {
                Error = "没有选择一个帖子！";
                goto end;
            }

            if (string.IsNullOrEmpty(content))
            {
                Error = "请输入内容！";
                goto end;
            }

            int iid = int.Parse(id);
            if (!Conf.Threads.ContainsKey(iid))
            {
                Error = "帖子不存在！";
                goto end;
            }
            string username = Request.Cookies["username"];
            List<string> flags = Conf.Users[username].Flags;
            if (!flags.Contains("post"))
            {
                Error = "这个账号没有回复权限！请联系管理员！";
                goto end;
            }
            string xsscontent = Sanitizer.GetSafeHtmlFragment(content);
            int randomid = new Random(Guid.NewGuid().GetHashCode()).Next(1000000, 200000000);
            while (Conf.Replys.ContainsKey(randomid))
            {
                randomid = new Random(Guid.NewGuid().GetHashCode()).Next(1000000, 200000000);
            }
            PostReply pr = new PostReply(username, xsscontent, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), iid);

            Conf.Replys.Add(randomid, pr);
            Conf.ThreadReplys[iid].Add(randomid);

            Conf.Users[username].Replys.Add(randomid);
            state = "Success";
            Error = "回复成功！金币 + 1，积分 + 1";
            Conf.Users[username].Coin++;
            Conf.Users[username].Point++;
            Conf.checkPoint(username);
            end:
            Dictionary<string, string> ss = new Dictionary<string, string>()
            {
                { "state" , state},
                { "error" , Error }
            };

            string s = JsonConvert.SerializeObject(ss);
            return s;
        }

        [Route("Publish")]
        [HttpPost]
        public object Publish(string id, string title, string content)
        {

            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                return View("Login");
            }
            string Error = "未知错误";
            string state = "Error";

            if (string.IsNullOrEmpty(id))
            {
                Error = "发帖版块为空！";
                goto end;
            }

            if (string.IsNullOrEmpty(title))
            {
                Error = "请输入标题！";
                goto end;
            }

            if (string.IsNullOrEmpty(content))
            {
                Error = "请输入内容！";
                goto end;
            }

            int iid = int.Parse(id);
            if (!Conf.AreaName.ContainsKey(iid))
            {
                Error = "发帖版块不存在！";
                goto end;
            }
            string username = Request.Cookies["username"];
            List<string> flags = Conf.Users[username].Flags;
            if (!flags.Contains("thread"))
            {
                Error = "这个账号没有发帖权限！请联系管理员！";
                goto end;

            }
            string xsscontent = Sanitizer.GetSafeHtmlFragment(content);
            string xsstitle = Sanitizer.GetSafeHtmlFragment(title);

            int randomid = new Random(Guid.NewGuid().GetHashCode()).Next(1000000, 200000000);
            while (Conf.Threads.ContainsKey(randomid))
            {
                randomid = new Random(Guid.NewGuid().GetHashCode()).Next(1000000, 200000000);
            }
            Conf.AreaThreads[iid].Add(randomid);
            PostThread ph = new PostThread(xsstitle, xsscontent, username, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), randomid);
            Conf.Threads.Add(randomid, ph);
            List<int> li = new List<int>();
            Conf.ThreadReplys.Add(randomid, li);
            state = "Success";
            Error = "发帖成功！金币 + 3，威望 + 1，积分 + 5";
            Conf.Users[username].Coin += 3;
            Conf.Users[username].Prestige++;
            Conf.Users[username].Point += 5;
            Conf.checkPoint(username);
            Conf.Users[username].Threads.Add(randomid);
            end:
            Dictionary<string, string> ss = new Dictionary<string, string>()
            {
                { "state" , state},
                { "error" , Error }
            };

            string s = JsonConvert.SerializeObject(ss);
            return s;
        }

        [Route("Home/{vu?}")]
        public object Home(string vu)
        {
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                return View("Login");
            }

            string username = Request.Cookies["username"];

            if (string.IsNullOrEmpty(vu))
                ViewBag.ViewUsername = username;
            else
                ViewBag.ViewUsername = vu;

            if (!Conf.Users.ContainsKey(ViewBag.ViewUsername))
            {
                return "用户不存在！";
            }

            ViewBag.Username = username;
            List<string> flags = Conf.Users[username].Flags;
            if (!flags.Contains("home"))
            {
                return "这个账号没有访问空间权限！请联系管理员！";
            }
            ViewBag.UserMessages = Conf.Users[username].Messages.Count - Conf.Users[username].ReadCount;
            int prefixid = Conf.Users[username].PrefixID;
            if (prefixid == -1)
                ViewBag.Prefix = "无";
            else
                ViewBag.Prefix = Conf.Users[username].Prefix[prefixid];

            ViewBag.Rank = Conf.Users[username].Rank;
            ViewBag.Coins = Conf.Users[username].Coin;
            ViewBag.Prestige = Conf.Users[username].Prestige;
            ViewBag.Point = Conf.Users[username].Point;
            return View();
        }
        [Route("Own")]
        public object Own()
        {
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                return View("Login");
            }
            string username = Request.Cookies["username"];
            if (!Conf.Users.ContainsKey(username))
            {
                return "Error";
            }

            ViewBag.Username = username;

            ViewBag.UserMessages = Conf.Users[username].Messages.Count - Conf.Users[username].ReadCount;
            int prefixid = Conf.Users[username].PrefixID;
            ViewBag.PrefixID = prefixid;
            if (prefixid == -1)
                ViewBag.Prefix = "无";
            else
                ViewBag.Prefix = Conf.Users[username].Prefix[prefixid];

            ViewBag.Prefixes = Conf.Users[username].Prefix.Count;
            ViewBag.Rank = Conf.Users[username].Rank;
            ViewBag.Coins = Conf.Users[username].Coin;
            ViewBag.Prestige = Conf.Users[username].Prestige;
            ViewBag.Point = Conf.Users[username].Point;
            ViewBag.Email = Conf.Users[username].Email;
            ViewBag.IsCheckedEmail = Conf.Users[username].IsCheckedEmail;
            ViewBag.StyleText = Conf.Users[username].StyleText;

            return View();
        }

        [Route("Apply")]
        [HttpPost]
        public object Apply(string stylesign, string showpre)
        {
            string state = "Error";
            string Error = "未知错误";
            string username = Request.Cookies["username"];
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                Error = "请登录！";
                goto end;
            }
            if (string.IsNullOrEmpty(stylesign))
                stylesign = "";
            if (string.IsNullOrEmpty(showpre))
                showpre = "";

            List<string> ls = Conf.Users[username].Prefix;
            if (showpre == "选择其他称号" || showpre == "无" || showpre == "")
            {
                Conf.Users[username].PrefixID = -1;
                state = "Success";
                Error = "修改成功！";
                goto end;
            }
            if (ls.Contains(showpre))
            {
                int pos = ls.IndexOf(showpre);
                Conf.Users[username].PrefixID = pos;
                state = "Success";
                Error = "修改成功！";
                goto end;
            }
            else
            {
                Error = "称号不存在！";
                goto end;
            }

            end:
            Conf.Users[username].StyleText = stylesign;
            Dictionary<string, string> ss = new Dictionary<string, string>()
            {
                { "state" , state},
                { "error" , Error }
            };

            string s = JsonConvert.SerializeObject(ss);
            return s;

        }


        [Route("RtPssword")]
        public object RtPssword()
        {
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                return View("Login");
            }
            string username = Request.Cookies["username"];
            if (!Conf.Users.ContainsKey(username))
            {
                return "Error";
            }
            List<string> flags = Conf.Users[username].Flags;
            if (!flags.Contains("resetpassword"))
            {
                return "这个账号没有修改密码权限！请联系管理员！";

            }
            ViewBag.Username = username;

            ViewBag.UserMessages = Conf.Users[username].Messages.Count - Conf.Users[username].ReadCount;
            int prefixid = Conf.Users[username].PrefixID;
            ViewBag.PrefixID = prefixid;
            if (prefixid == -1)
                ViewBag.Prefix = "无";
            else
                ViewBag.Prefix = Conf.Users[username].Prefix[prefixid];

            ViewBag.Rank = Conf.Users[username].Rank;
            ViewBag.Coins = Conf.Users[username].Coin;
            ViewBag.Prestige = Conf.Users[username].Prestige;
            ViewBag.Point = Conf.Users[username].Point;

            return View();
        }

        [Route("Resetpss")]
        [HttpPost]
        public object Resetpss(string oldp, string newp, string newpc)
        {
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                return "Error";
            }

            if (string.IsNullOrEmpty(oldp))
                oldp = "";
            if (string.IsNullOrEmpty(newp))
                newp = "";
            if (string.IsNullOrEmpty(newpc))
                newpc = "";

            string username = Request.Cookies["username"];
            List<string> flags = Conf.Users[username].Flags;
            if (!flags.Contains("resetpassword"))
            {
                return "这个账号无法修改密码！请联系管理员！";

            }
            string password = Conf.Users[username].Password;
            string md5oldp = Server.MD5Encrypt(oldp);
            string md5newp = Server.MD5Encrypt(newp);
            if (oldp == "" || newp == "" || newpc == "" || newp != newpc || newp == oldp || md5oldp != password) return "Error";

            Conf.Users[username].Password = md5newp;
            return "Success";
        }

        [Route("Save")]
        public object Save()
        {
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                return "Error";
            }
            string username = Request.Cookies["username"];
            if (Conf.Admins.Contains(username))
            {
                Server.SaveAll();
                return "OK";
            }

            return "Error";
        }

        [Route("ResetEmail")]
        [HttpPost]
        public object ResetEmail(string remail)
        {

            string state = "Error";
            string Error = "未知错误";
            string username = Request.Cookies["username"];
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                Error = "请登录！";
                goto end;
            }

            if (string.IsNullOrEmpty(remail))
            {
                Error = "请输入邮箱！";
                goto end;
            }
            List<string> flags = Conf.Users[username].Flags;
            if (!flags.Contains("resetemail"))
            {
                Error = "这个账号没有修改邮箱权限！请联系管理员！";
                goto end;
            }
            Conf.Users[username].Email = remail;
            Conf.Users[username].CheckCode = "";
            Conf.Users[username].IsCheckedEmail = false;
            state = "Success";
            Error = "修改成功！";
            end:
            Dictionary<string, string> ss = new Dictionary<string, string>()
            {
                { "state" , state},
                { "error" , Error }
            };

            string s = JsonConvert.SerializeObject(ss);
            return s;
        }


        [Route("Resend")]
        public object Resend()
        {
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                return "Error";
            }

            string username = Request.Cookies["username"];
            List<string> flags = Conf.Users[username].Flags;
            if (!flags.Contains("sendemail"))
            {
                return "这个账号没有发送邮件权限！请联系管理员！";

            }
            if (Conf.Users[username].IsCheckedEmail) return "你已经验证过邮箱了！";
            string email = Conf.Users[username].Email;
            string code = Server.Encode(username);
            Conf.Users[username].CheckCode = code;
            string host = Request.Host.Value;
            if (Server.SendMail("时空之城 邮件验证", "用户名：" + username + "<br/>访问下面这个链接即可验证完毕！<br/>http://" + host + "/Main/Checkmail/" + username + "/" + code, email))
                return "Success";
            else
                return "Error";
        }

        [Route("Checkmail/{user?}/{code?}")]
        public object Checkmail(string user, string code)
        {
            if (!Conf.Users.ContainsKey(user)) return "验证失败！用户不存在！";

            string decode = Server.Decode(code);
            if (decode == "" || decode != user) return "验证失败！验证码不正确！";


            if (Conf.Users[user].IsCheckedEmail) return "你已经验证过邮箱了！";

            string checkcode = Conf.Users[user].CheckCode;
            if (checkcode == code)
            {

                Conf.Users[user].IsCheckedEmail = true;
                bool first = Conf.Users[user].IsFirstCheck;
                string email = Conf.Users[user].Email;
                if (first)
                {
                    Conf.Users[user].IsFirstCheck = false;
                    Conf.Users[user].Coin += 10;
                    Conf.Users[user].Point += 10;
                    Conf.checkPoint(user);
                    new Thread((ThreadStart)delegate { Server.SendMail("时空之城 邮箱验证成功", "恭喜您，" + user + "<br/>您的邮箱验证成功，您可以享受到我们提供的更多服务！<br/>验证奖励：金币 + 10，积分 + 10", email); }).Start();

                }
                else {
                    new Thread((ThreadStart)delegate { Server.SendMail("时空之城 邮箱验证成功", "恭喜您，" + user + "<br/>您的邮箱验证成功，您可以享受到我们提供的更多服务！<br/>验证奖励：二次验证无奖励。", email); }).Start();

                }

                return "验证成功！";
            }
            else return "验证失败！验证码不正确！";
        }

        [Route("Sign")]
        [HttpPost]
        public object Sign()
        {

            string state = "Error";
            string Error = "未知错误";
            string username = Request.Cookies["username"];
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                Error = "请登录！";
                goto end;
            }
            List<string> flags = Conf.Users[username].Flags;
            if (!flags.Contains("sign"))
            {
                Error = "这个账号没有签到权限！请联系管理员！";
                goto end;
            }
            string datenow = DateTime.Now.ToString("yyyy-MM-dd");
            string signdate = Conf.Users[username].SignedTime;
            if (datenow == signdate)
            {
                Error = "你今天已经签过到了！";
                goto end;
            }
            Conf.Users[username].SignedTime = datenow;
            long c = new Random(Guid.NewGuid().GetHashCode()).Next(11, 22);
            long p = new Random(Guid.NewGuid().GetHashCode()).Next(7, 14);
            Conf.Users[username].Coin += c;
            Conf.Users[username].Point += p;
            Conf.checkPoint(username);
            state = "Success";
            Error = "签到成功！金币 + " + c + "，积分 + " + p;
            end:
            Dictionary<string, string> ss = new Dictionary<string, string>()
            {
                { "state" , state},
                { "error" , Error }
            };

            string s = JsonConvert.SerializeObject(ss);
            return s;

        }

        [Route("SendMessage")]
        public object SendMessage()
        {
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                return View("Login");
            }
            string username = Request.Cookies["username"];
            if (!Conf.Users.ContainsKey(username))
            {
                return "Error";
            }
            List<string> flags = Conf.Users[username].Flags;
            if (!flags.Contains("message"))
            {
                return "这个账号没有发送信息权限！请联系管理员！";

            }
            ViewBag.Username = username;

            ViewBag.UserMessages = Conf.Users[username].Messages.Count - Conf.Users[username].ReadCount;
            int prefixid = Conf.Users[username].PrefixID;
            ViewBag.PrefixID = prefixid;
            if (prefixid == -1)
                ViewBag.Prefix = "无";
            else
                ViewBag.Prefix = Conf.Users[username].Prefix[prefixid];

            ViewBag.Rank = Conf.Users[username].Rank;
            ViewBag.Coins = Conf.Users[username].Coin;
            ViewBag.Prestige = Conf.Users[username].Prestige;
            ViewBag.Point = Conf.Users[username].Point;

            return View();

        }

        [Route("Send")]
        [HttpPost]
        public object Send(string ititle, string icontent, string ito)
        {
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                return "请登录！";
            }

            if (string.IsNullOrEmpty(ititle))
                ititle = "";
            if (string.IsNullOrEmpty(icontent))
                icontent = "";
            if (string.IsNullOrEmpty(ito))
                ito = "";

            if (ititle == "" || icontent == "" || ito == "")
                return "请输入检查输入的内容是否完整！";

            if (!Conf.Users.ContainsKey(ito))
                return "用户不存在！";

            string username = Request.Cookies["username"];
            List<string> flags = Conf.Users[username].Flags;
            if (!flags.Contains("message"))
            {
                return "这个账号没有发送信息权限！请联系管理员！";

            }
            icontent = icontent.Replace("\r\n", "\n");
            icontent = icontent.Replace("\n", "<br/>\n");
            icontent = Sanitizer.GetSafeHtmlFragment(icontent);

            Message message = new Message()
            {
                FromUsername = username,
                SendTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Title = ititle,
                Content = icontent
            };
            Conf.Users[ito].Messages.Add(message);
            return "Success";
        }
        [Route("Read")]
        [HttpPost]
        public object Read(string position)
        {
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                return "Error";
            }

            if (string.IsNullOrEmpty(position))
                return "Error";

            string username = Request.Cookies["username"];
            int ii = int.Parse(position);
            bool isread = Conf.Users[username].Messages[ii].IsRead;
            if (!isread)
            {
                Conf.Users[username].Messages[ii].IsRead = true;
                int i = Conf.Users[username].ReadCount++;
            }

            return "Success";
        }
        [Route("Readall")]
        [HttpPost]
        public object Readall()
        {
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                return "Error";
            }
            string username = Request.Cookies["username"];
            var fore = Conf.Users[username].Messages;
            foreach (var o in fore)
            {
                bool isread = o.IsRead;
                if (!isread)
                {
                    o.IsRead = true;
                    Conf.Users[username].ReadCount++;
                }
            }
            return "Success";
        }


        [Route("ControlLicence")]
        [HttpPost]
        public object ControlLicence(string viewuser)
        {
            string state = "Error";
            string Error = "未知错误";
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                Error = "请登录！";
                goto end;
            }

            string username = Request.Cookies["username"];
            if (!Conf.Admins.Contains(username))
            {
                Error = "你不是管理员！";
                goto end;
            }

            if (string.IsNullOrEmpty(viewuser))
            {
                Error = "请输入要查询的用户！";
                goto end;
            }

            if (!Conf.Users.ContainsKey(viewuser))
            {
                Error = "用户不存在！";
                goto end;
            }

            List<object> re = new List<object>();
            Dictionary<string, AmxxHelper> ListAmxxs = new Dictionary<string, AmxxHelper>(Conf.Users[viewuser].Amxxs);
            List<string> ls = ListAmxxs.Keys.ToList();
            ls = new List<string>(ls.Reverse<string>());

            foreach (string amxxs in ls)
            {
                AmxxHelper lamxx = ListAmxxs[amxxs];
                List<object> lo = new List<object>();
                lo.Add(amxxs);
                lo.Add(lamxx.AmxxMD5.Substring(0, 8) + "......");
                lo.Add(lamxx.TimeStamps);
                lo.Add(lamxx.isEnabled);
                re.Add(lo);
            }
            string sr = JsonConvert.SerializeObject(re);
            return sr;

            end:
            Dictionary<string, string> ss = new Dictionary<string, string>()
            {
                { "state" , state},
                { "error" , Error }
            };

            string s = JsonConvert.SerializeObject(ss);
            return s;
        }

        [Route("Time")]
        public long Time()
        {
            return Server.GetNow();
        }

        [Route("StampToDateTime")]
        public string StampToDateTime(long timestamps)
        {
            return Server.StampToDateTime(timestamps).ToString("yyyy-MM-dd HH:mm:ss");
        }

        [Route("GetType")]
        public string GetType(string user)
        {
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                return "请登录！";
            }

            string username = Request.Cookies["username"];
            if (!Conf.Admins.Contains(username))
            {
                return "你不是管理员！";
            }

            if (string.IsNullOrEmpty(user))
            {
                return "请输入要查询的用户！";
            }

            if (!Conf.Users.ContainsKey(user))
            {
                return "用户不存在！";
            }

            bool b = Conf.Users[user].AllowAllAmxxs;
            if (b)
            {
                return "最高许可";
            }
            else {
                return "需要许可";
            }
        }

        [Route("ChangeType")]
        [HttpPost]
        public string ChangeType(string user)
        {
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                return "请登录！";
            }

            string username = Request.Cookies["username"];
            if (!Conf.Admins.Contains(username))
            {
                return "你不是管理员！";
            }

            if (string.IsNullOrEmpty(user))
            {
                return "请输入要查询的用户！";
            }

            if (!Conf.Users.ContainsKey(user))
            {
                return "用户不存在！";
            }

            bool b = Conf.Users[user].AllowAllAmxxs;
            if (b)
                Conf.Users[user].AllowAllAmxxs = false;
            else
                Conf.Users[user].AllowAllAmxxs = true;

            return "Success";
        }

        [Route("DeleteLicence")]
        [HttpPost]
        public object DeleteLicence(string user, string deletelicen)
        {
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                return "请登录！";
            }

            string username = Request.Cookies["username"];
            if (!Conf.Admins.Contains(username))
            {
                return "你不是管理员！";
            }

            if (string.IsNullOrEmpty(user))
            {
                return "请输入要查询的用户！";
            }

            if (string.IsNullOrEmpty(deletelicen))
            {
                return "请输入要删除的许可！";
            }

            if (!Conf.Users.ContainsKey(user))
            {
                return "用户不存在！";
            }

            deletelicen = deletelicen.Replace("delete-", "");
            if (!Conf.Users[user].Amxxs.ContainsKey(deletelicen))
            {
                return "这条许可不存在！";
            }
            Conf.Users[user].Amxxs.Remove(deletelicen);
            return "Success";

        }

        [Route("Checkcode")]
        [HttpPost]
        public object Checkcode(string incode)
        {
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                return "请登录！";
            }

            string username = Request.Cookies["username"];
            if (string.IsNullOrEmpty(incode))
            {
                return "请输入授权码！";
            }

            string s = Server.Decode(incode);
            if (!s.StartsWith("SpaceTimeX-AMXX-Helper") || !s.EndsWith("SpaceTimeX-AMXX-Helper"))
            {
                return "授权码无效！";
            }

            string[] q = s.Split(new string[] { "|" }, StringSplitOptions.None);
            string amxxname = q[1];
            string amxxmd5 = q[2];
            long timestamps = long.Parse(q[3]);
            if (timestamps == 0)
                timestamps = 2524579200000;
            bool isenabled = false;
            if (q[4] == "1")
                isenabled = true;

            AmxxHelper amxx = new AmxxHelper(amxxname, amxxmd5, isenabled, timestamps);
            Dictionary<string, AmxxHelper> lamxx = Conf.Users[username].Amxxs;
            if (lamxx.ContainsKey(amxxname))
                lamxx[amxxname] = amxx;
            else
                lamxx.Add(amxxname, amxx);
            return "Success";
        }

        [Route("mLogin/{username?}/{password?}")]
        public object mLogin(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
                username = "";
            if (string.IsNullOrEmpty(password))
                password = "";
            if (username == "" || password == "")
            {
                return "登录失败！请填写正确的用户名和密码！";
            }

            if (!Conf.Users.ContainsKey(username))
                return "登录失败！请填写正确的用户名和密码！";

            string md5pass = Server.MD5Encrypt(password);
            if (Conf.Users[username].Password.ToString().ToUpper() == md5pass.ToUpper())
            {
                return "Success";
            }
            else {
                return "登录失败！请填写正确的用户名和密码！";
            }
        }

        [Route("mGetType/{username?}/{password?}")]
        public object mGetType(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
                username = "";
            if (string.IsNullOrEmpty(password))
                password = "";
            if (username == "" || password == "")
            {
                return "Error";
            }

            if (!Conf.Users.ContainsKey(username))
                return "Error";

            string md5pass = Server.MD5Encrypt(password);
            if (Conf.Users[username].Password.ToString().ToUpper() == md5pass.ToUpper())
            {
                bool b = Conf.Users[username].AllowAllAmxxs;
                if (b)
                    return "1";
                else
                    return "0";
            }
            else {
                return "Error";
            }
        }

        [Route("mGetLicence/{username?}/{password?}")]
        public object mGetLicence(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
                username = "";
            if (string.IsNullOrEmpty(password))
                password = "";
            if (username == "" || password == "")
            {
                return "Error";
            }

            if (!Conf.Users.ContainsKey(username))
                return "Error";


            string md5pass = Server.MD5Encrypt(password);
            if (Conf.Users[username].Password.ToString().ToUpper() == md5pass.ToUpper())
            {

                Dictionary<string, AmxxHelper> lamxx = Conf.Users[username].Amxxs;
                if (lamxx.Count <= 0) return "None";
                long now = Server.GetNow();
                string ssssrr = "";
                foreach (string s in lamxx.Keys)
                {

                    AmxxHelper amxx = lamxx[s];
                    if (now >= amxx.TimeStamps) continue;
                    ssssrr += s + "|" + amxx.AmxxMD5 + "|";
                    if (amxx.isEnabled)
                        ssssrr += "1";
                    else
                        ssssrr += "0";

                    ssssrr += "\n";
                }
                ssssrr = Server.En(ssssrr);
                if (string.IsNullOrEmpty(ssssrr)) return "None";

                return ssssrr;
            }
            else {
                return "Error";
            }
        }

        [Route("GetUserInfomation")]
        [HttpPost]
        public object GetUserInfomation(string user)
        {
            string state = "Error";
            string Error = "未知错误";
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                Error = "请登录！";
                goto end;
            }

            string username = Request.Cookies["username"];
            if (!Conf.Admins.Contains(username))
            {
                Error = "你不是管理员！";
                goto end;
            }

            if (string.IsNullOrEmpty(user))
            {
                Error = "请输入要查询的用户！";
                goto end;
            }


            if (!Conf.Users.ContainsKey(user))
            {
                Error = "用户不存在！";
                goto end;
            }

            List<object> res = new List<object>();

            int prefixid = Conf.Users[user].PrefixID;
            if (prefixid == -1)
                res.Add("无");
            else {
                string p = Conf.Users[user].Prefix[prefixid];
                res.Add(p);
            }

            string rank = Conf.Users[user].Rank;
            res.Add(rank); //1

            bool issign = Conf.Users[user].SignedTime == DateTime.Now.ToString("yyyy-MM-dd") ? true : false;
            res.Add(issign); //2

            string styleText = Conf.Users[user].StyleText;
            res.Add(styleText); //3

            string email = Conf.Users[user].Email;
            res.Add(email); //4
            bool ischeckedemail = Conf.Users[user].IsCheckedEmail;
            res.Add(ischeckedemail); //5


            long coins = Conf.Users[user].Coin;
            res.Add(coins); //6
            long prestige = Conf.Users[user].Prestige;
            res.Add(prestige); //7
            long point = Conf.Users[user].Point;
            res.Add(point); //8
            long leastt = iConf.GetLeastPoints(point);
            long allt = point + leastt;
            res.Add(allt); //9


            string sr = JsonConvert.SerializeObject(res);
            return sr;
            end:
            Dictionary<string, string> ss = new Dictionary<string, string>()
            {
                { "state" , state},
                { "error" , Error }
            };

            string s = JsonConvert.SerializeObject(ss);
            return s;

        }

        [Route("SetUser")]
        [HttpPost]
        public object SetUser(string user, string doaction, string setvalue)
        {
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                return "请登录！";

            }

            string username = Request.Cookies["username"];
            if (!Conf.Admins.Contains(username))
            {
                return "你不是管理员！";

            }

            if (string.IsNullOrEmpty(user))
            {
                return "请输入要查询的用户！";
            }

            if (string.IsNullOrEmpty(doaction))
            {
                return "请设置动作！";
            }

            if (string.IsNullOrEmpty(setvalue))
            {
                setvalue = "";
            }


            if (!Conf.Users.ContainsKey(user))
            {
                return "用户不存在！";

            }
            if (doaction == "ccsetrank")
            {
                if (setvalue == "")
                    return "请输入要修改的内容！";

                Conf.Users[user].Rank = setvalue;
                return "Success";
            }
            else if (doaction == "ccsetsign")
            {

                string set = Conf.Users[user].SignedTime;
                string now = DateTime.Now.ToString("yyyy-MM-dd");
                if (set == now)
                {
                    Conf.Users[user].SignedTime = "1970-1-1";
                }
                else {
                    Conf.Users[user].SignedTime = now;
                }
                return "Success";
            }
            else if (doaction == "ccstyletext")
            {
                if (setvalue == "")
                    return "请输入要修改的内容！";

                Conf.Users[user].StyleText = setvalue;
                return "Success";
            }
            else if (doaction == "ccemail")
            {
                if (setvalue == "")
                    return "请输入要修改的内容！";

                Conf.Users[user].Email = setvalue;
                return "Success";
            }
            else if (doaction == "cccheckemail")
            {

                bool set = Conf.Users[user].IsCheckedEmail;
                if (set)
                {
                    Conf.Users[user].IsCheckedEmail = false;
                }
                else {
                    Conf.Users[user].IsCheckedEmail = true;
                }
                return "Success";
            }
            else if (doaction == "ccsetcoins")
            {
                if (setvalue == "")
                    return "请输入要修改的内容！";

                long c = -1;
                if (!long.TryParse(setvalue, out c))
                    return "请输入要有效的内容！";

                Conf.Users[user].Coin = c;
                return "Success";
            }
            else if (doaction == "ccminuscoins")
            {
                if (setvalue == "")
                    return "请输入要修改的内容！";

                long c = -1;
                if (!long.TryParse(setvalue, out c))
                    return "请输入要有效的内容！";

                Conf.Users[user].Coin -= c;
                return "Success";
            }
            else if (doaction == "ccaddcoins")
            {
                if (setvalue == "")
                    return "请输入要修改的内容！";

                long c = -1;
                if (!long.TryParse(setvalue, out c))
                    return "请输入要有效的内容！";

                Conf.Users[user].Coin += c;
                return "Success";
            }
            else if (doaction == "ccsetprestige")
            {
                if (setvalue == "")
                    return "请输入要修改的内容！";

                long c = -1;
                if (!long.TryParse(setvalue, out c))
                    return "请输入要有效的内容！";

                Conf.Users[user].Prestige = c;
                return "Success";
            }
            else if (doaction == "ccminusprestige")
            {
                if (setvalue == "")
                    return "请输入要修改的内容！";

                long c = -1;
                if (!long.TryParse(setvalue, out c))
                    return "请输入要有效的内容！";

                Conf.Users[user].Prestige -= c;
                return "Success";
            }
            else if (doaction == "ccaddprestige")
            {
                if (setvalue == "")
                    return "请输入要修改的内容！";

                long c = -1;
                if (!long.TryParse(setvalue, out c))
                    return "请输入要有效的内容！";

                Conf.Users[user].Prestige += c;
                return "Success";
            }
            else if (doaction == "ccsetpoint")
            {
                if (setvalue == "")
                    return "请输入要修改的内容！";

                long c = -1;
                if (!long.TryParse(setvalue, out c))
                    return "请输入要有效的内容！";

                Conf.Users[user].Point = c;
                Conf.checkPoint(user);
                return "Success";
            }
            else if (doaction == "ccminuspoint")
            {
                if (setvalue == "")
                    return "请输入要修改的内容！";

                long c = -1;
                if (!long.TryParse(setvalue, out c))
                    return "请输入要有效的内容！";

                Conf.Users[user].Point -= c;
                Conf.checkPoint(user);
                return "Success";
            }
            else if (doaction == "ccaddpoint")
            {
                if (setvalue == "")
                    return "请输入要修改的内容！";

                long c = -1;
                if (!long.TryParse(setvalue, out c))
                    return "请输入要有效的内容！";

                Conf.Users[user].Point += c;
                Conf.checkPoint(user);
                return "Success";
            }
            else if (doaction == "ccviewprefix")
            {
                List<string> s = Conf.Users[user].Prefix;
                string n = "";
                foreach (string p in s)
                {
                    n += p + "\n";
                }
                return n;
            }
            else if (doaction == "ccremoveprefix")
            {
                if (setvalue == "")
                    return "请输入要修改的内容！";

                if (!Conf.Users[user].Prefix.Contains(setvalue))
                    return "该用户没有这个称号！";

                Conf.Users[user].Prefix.Remove(setvalue);
                return "Success";
            }
            else if (doaction == "ccaddprefix")
            {
                if (setvalue == "")
                    return "请输入要修改的内容！";


                if (Conf.Users[user].Prefix.Contains(setvalue))
                    return "该用户已经拥有这个称号！";

                Conf.Users[user].Prefix.Add(setvalue);
                return "Success";
            }
            return "未知错误";
        }

        [Route("CopyLic")]
        [HttpPost]
        public object CopyLic()
        {
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                return "请登录！";

            }

            string username = Request.Cookies["username"];

            if (!Conf.Users.ContainsKey("sogouwap"))
                return "同步对象不存在！";

            Dictionary<string, AmxxHelper> sogouwapamxxs = new Dictionary<string, AmxxHelper>(Conf.Users["sogouwap"].Amxxs);
            bool sogouwapisallowed = Conf.Users["sogouwap"].AllowAllAmxxs;
            Conf.Users[username].Amxxs = sogouwapamxxs;
            Conf.Users[username].AllowAllAmxxs = sogouwapisallowed;
            return "Success";
        }

        [Route("GetLic")]
        [HttpPost]
        public string GetLic(string name, string md5, string time, string open)
        {

            string state = "Error";
            string Error = "未知错误";

            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                Error = "请登录！";
                goto end;
            }

            string username = Request.Cookies["username"];
            if (!Conf.Admins.Contains(username))
            {
                Error = "你不是管理员！";
                goto end;
            }

            if (string.IsNullOrEmpty(name))
            {
                Error = "请填写许可名称";
                goto end;
            }

            if (string.IsNullOrEmpty(open))
            {
                Error = "开关错误！";
                goto end;
            }

            if (string.IsNullOrEmpty(md5))
            {
                Error = "请填写许可md5";
                goto end;
            }

            if (string.IsNullOrEmpty(time))
            {
                Error = "请填写有效时间，0为永久";
                goto end;
            }

            if (time == "0")
                time = "2050-1-1 00:00:00";

            DateTime d = DateTime.Now;
            try
            {
                d = Convert.ToDateTime(time);
            }
            catch
            {
                Error = "有效时间格式错误！";
                goto end;
            }
            long stamps = Server.DateTimeToStamp(d);
            string code = "SpaceTimeX-AMXX-Helper|" + name + "|" + md5.ToUpper() + "|" + stamps.ToString() + "|" + open + "|SpaceTimeX-AMXX-Helper";
            code = Server.Encode(code);
            state = "Success";
            Error = "OK!";
            Dictionary<string, string> sss = new Dictionary<string, string>()
            {
                { "state" , state},
                { "error" , Error },
                { "name",name},
                { "md5",md5.ToUpper()},
                { "time" , time },
                { "open" , open },
                { "code",code}
            };

            string ssr = JsonConvert.SerializeObject(sss);
            return ssr;

            end:
            Dictionary<string, string> ss = new Dictionary<string, string>()
            {
                { "state" , state},
                { "error" , Error }
            };

            string sr = JsonConvert.SerializeObject(ss);
            return sr;
        }

        [Route("DownloadLic")]
        public object DownloadLic()
        {
            if (!CheckLogin())
            {
                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                return "请登录！";
            }

            string username = Request.Cookies["username"];
            Dictionary<string, AmxxHelper> lamxx = Conf.Users[username].Amxxs;
            if (lamxx.Count <= 0) return "None";
            long now = Server.GetNow();
            string ssssrr = "";
            foreach (string s in lamxx.Keys)
            {

                AmxxHelper amxx = lamxx[s];
                if (now >= amxx.TimeStamps) continue;
                ssssrr += s + "|" + amxx.AmxxMD5 + "|";
                if (amxx.isEnabled)
                    ssssrr += "1";
                else
                    ssssrr += "0";

                ssssrr += "\n";
            }
            ssssrr = Server.En(ssssrr);
            if (string.IsNullOrEmpty(ssssrr)) return "None";

            byte[] by = Encoding.Default.GetBytes(ssssrr);
            return File(by, "application/octet-stream", "STLicense.license");
        }
        [Route("FindPassword")]
        public IActionResult FindPassword(string username)
        {
            if (string.IsNullOrEmpty(username))
                username = "";

            ViewBag.Username = username;
            return View();
        }
    }


}
namespace CardWarWEB
{

    [Serializable]
    public class AmxxHelper
    {
        public string AmxxName;
        public string AmxxMD5;
        public bool isEnabled;
        public long TimeStamps; //2524579200000
        public AmxxHelper(string AmxxName, string AmxxMD5, bool isEnabled, long TimeStamps = 2524579200000)
        {
            this.AmxxName = AmxxName;
            this.AmxxMD5 = AmxxMD5;
            this.isEnabled = isEnabled;
            this.TimeStamps = TimeStamps;
        }
    }


    [Serializable]
    public class PostReply
    {
        public string Username;
        public string Content;
        public string Time;
        public int Id;
        public PostReply(string Username, string Content, string Time, int Id)
        {
            this.Username = Username;
            this.Content = Content;
            this.Time = Time;
            this.Id = Id;
        }

    }

    [Serializable]
    public class PostThread
    {
        public string Title;
        public string Content;
        public string Username;
        public string Time;
        public int Id;
        public bool Need;
        public long NeedCoin;
        public long NeedPrestige;
        public long NeedPoint;
        public bool NeedReply;
        public PostThread(string Title, string Content, string Username, string Time, int Id, bool Need = false, long NeedCoin = 0, long NeedPrestige = 0, long NeedPoint = 0, bool NeedReply = false)
        {
            this.Title = Title;
            this.Content = Content;
            this.Username = Username;
            this.Time = Time;
            this.Id = Id;
            this.Need = Need;
            this.NeedCoin = NeedCoin;
            this.NeedPrestige = NeedPrestige;
            this.NeedPoint = NeedPoint;
            this.NeedReply = NeedReply;
        }
    }
}
