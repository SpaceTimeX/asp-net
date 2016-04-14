using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CardWarWEB
{
    public class Server
    {
        public static string MapPath(string path)
        {
            return Conf.env.MapPath(path);

        }

        public static void RemoveCookie(Controller c, string name)
        {
            c.Response.Cookies.Append(name, "", new Microsoft.AspNet.Http.CookieOptions()
            {
                Expires = DateTime.Now.AddDays(-1)
            });
        }
        public static string GetApplicationBasePath()
        {
            return Conf.appEnv.ApplicationBasePath;
        }

        public static string GetWebRootPath()
        {
            return Conf.env.WebRootPath;
        }

        public static string MD5Encrypt(string strText)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(strText, "MD5").ToUpper();
        }

        //是否是用户名:\w表示数字，英文大小写字母，下划线的组合，即匹配包括下划线的单词字符
        public static bool IsUsername(string name)
        {
            Regex rgx = new Regex("^[\\w]+$");
            return rgx.IsMatch(name);
        }

        //\W表示非单词字符，密码的字符一般根据实际需要选择范围
        //^[\d]+$ 纯数字密码
        //^[\da-z]+$ 数字+小写字母
        //^[\w%!]+$ 单词字符+某些特殊字符
        public static bool IsPassword(string pwd)
        {
            Regex rgx = new Regex("^[\\w\\W]+$");//密码范围广
            return rgx.IsMatch(pwd);
        }


        public static void SaveUsersConf()
        {
            if (Directory.Exists("/v"))
            {
                Directory.CreateDirectory("/v/Users");
                FileStream fs = new FileStream("/v/Users.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, Conf.Users);
                fs.Close();
            }
            else
            {
                Directory.CreateDirectory(GetApplicationBasePath() + "/Users");
                FileStream fs = new FileStream(GetApplicationBasePath() + "/Users.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, Conf.Users);
                fs.Close();
            }

        }

        public static void SaveThreads()
        {
            if (Directory.Exists("/v"))
            {
                Directory.CreateDirectory("/v/Users");
                FileStream fs = new FileStream("/v/Threads.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, Conf.Threads);
                fs.Close();
            }
            else
            {
                Directory.CreateDirectory(GetApplicationBasePath() + "/Users");
                FileStream fs = new FileStream(GetApplicationBasePath() + "/Threads.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, Conf.Threads);
                fs.Close();
            }

        }

        public static void SaveReplys()
        {
            if (Directory.Exists("/v"))
            {
                Directory.CreateDirectory("/v/Users");
                FileStream fs = new FileStream("/v/Replys.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, Conf.Replys);
                fs.Close();
            }
            else
            {
                Directory.CreateDirectory(GetApplicationBasePath() + "/Users");
                FileStream fs = new FileStream(GetApplicationBasePath() + "/Replys.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, Conf.Replys);
                fs.Close();
            }

        }

        public static void SaveThreadReplys()
        {
            if (Directory.Exists("/v"))
            {
                Directory.CreateDirectory("/v/Users");
                FileStream fs = new FileStream("/v/ThreadReplys.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, Conf.ThreadReplys);
                fs.Close();
            }
            else
            {
                Directory.CreateDirectory(GetApplicationBasePath() + "/Users");
                FileStream fs = new FileStream(GetApplicationBasePath() + "/ThreadReplys.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, Conf.ThreadReplys);
                fs.Close();
            }

        }

        public static void SaveEmails()
        {
            if (Directory.Exists("/v"))
            {
                Directory.CreateDirectory("/v/Users");
                FileStream fs = new FileStream("/v/Emails.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, Conf.Emails);
                fs.Close();
            }
            else
            {
                Directory.CreateDirectory(GetApplicationBasePath() + "/Users");
                FileStream fs = new FileStream(GetApplicationBasePath() + "/Emails.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, Conf.Emails);
                fs.Close();
            }

        }

        public static void SaveAll()
        {
            try
            {
                SaveUsersConf();
                SaveThreads();
                SaveReplys();
                SaveThreadReplys();
                SaveEmails();
            }
            catch (Exception e)
            {
                File.WriteAllText("./ErrorMessage.txt", e.Message);
                File.WriteAllText("./ErrorStackTrace.txt", e.StackTrace);
            }
        }

        [DllImport(".\\src\\CardWarWEB\\Mail.dll")]
        extern static bool SendMail(IntPtr Title, IntPtr Content, IntPtr SendTo);
        [DllImport(".\\src\\CardWarWEB\\Mail.dll")]
        extern static IntPtr Encode(IntPtr username);
        [DllImport(".\\src\\CardWarWEB\\Mail.dll")]
        extern static IntPtr Decode(IntPtr code);
        [DllImport(".\\src\\CardWarWEB\\Mail.dll")]
        static extern IntPtr De(IntPtr content);
        [DllImport(".\\src\\CardWarWEB\\Mail.dll")]
        static extern IntPtr En(IntPtr content);
        public static bool SendMail2(string Title, string Content, string SendTo)
        {

            IntPtr iTitle = sg(Title);
            IntPtr iContent = sg(Content);
            IntPtr iSendTo = sg(SendTo);
            return SendMail(iTitle, iContent, iSendTo);
        }

        public static bool SendMail(string Title, string Content, string SendTo) {

            Email email = new Email("127.0.0.1", SendTo, "时空之主<ms@mainspace.ml>", Title, Content, "ms", "990131", "25", false, true);
            return email.Send();
        }

        public static string De(string content)
        {
            IntPtr icontent = sg(content);
            return gs(De(icontent));

        }
        public static string En(string content)
        {
            IntPtr icontent = sg(content);
            return gs(En(icontent));

        }
        public static string Encode(string username) {
            IntPtr iusername = sg(username);
            IntPtr res = Encode(iusername);
            string s = gs(res);
            return s;
        }

        public static string Decode(string code)
        {
            IntPtr icode = sg(code);
            IntPtr res = Decode(icode);
            string s = gs(res);
            return s;
        }

        public static string gs(IntPtr ptrRet)
        {
            string retlust = Marshal.PtrToStringAnsi(ptrRet);
            return retlust;
        }

        public static IntPtr sg(string s)
        {
            IntPtr ptrIn = Marshal.StringToHGlobalAnsi(s);
            return ptrIn;
        }

        public static DateTime StampToDateTime(long timeStamp)
        {
            DateTime dateTimeStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = timeStamp * 10000;
            TimeSpan toNow = new TimeSpan(lTime);
            return dateTimeStart.Add(toNow);
        }
        public static long DateTimeToStamp(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }
        public static long GetNow()
        {
            return DateTimeToStamp(DateTime.Now);

        }
    }
}
