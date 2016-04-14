using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardWarWEB
{
    [Serializable]
    public class UserConfig {
        public string Username;
        public string Password;
        public string Email;
        public List<string> Flags = new List<string>() { "post", "thread", "home", "login", "resetpassword", "resetemail",
                                   "view", "message", "styletext","getcoins","getprestige","getpoint","sendemail","sign","mainpage","apply","uploadimage" };
        public string Rank = "信徒";
        public string StyleText = "";
        public List<string> Prefix = new List<string>() { "欢乐的SB", "愉悦的草泥马", "妈的智障", "这...那...我..." };
        public int PrefixID = -1;
        public long Coin = 10;
        public long Prestige = 0;
        public long Point = 0;
        public List<Message> Messages = new List<Message>();
        public List<int> Threads = new List<int>();
        public List<int> Replys = new List<int>();
        public string CheckCode = "";
        public string FindPasswordCode = "";
        public bool IsCheckedEmail = false;
        public bool IsFirstCheck = true;
        public string SignedTime = "1970-1-1";
        public int ReadCount = 0;
        public bool AllowAllAmxxs = false;
        public Dictionary<string, AmxxHelper> Amxxs = new Dictionary<string, AmxxHelper>();

    }

    [Serializable]
    public class Message {
        public string FromUsername;
        public string SendTime;
        public string Title;
        public string Content;
        public bool IsRead = false;
    }



    public class Conf
    {
        public static IHostingEnvironment env;
        public static IApplicationEnvironment appEnv;

        public static Dictionary<string, string> Permissions = new Dictionary<string, string>() {
            { "post","回复"},
            { "thread","发帖"},
            { "home","访问空间"},
            { "login","登录"},
            { "resetpassword","修改密码"},
            { "resetemail","修改邮箱"},
            { "sendemail","发送邮件"},
            { "sign","签到"},
            { "view","浏览帖子"},
            { "sell","发起交易"},
            { "vote","发起投票"},
            { "message","发信息"},
            { "styletext","修改签名"},
            { "deleteself","删除自己的帖子"},
            { "deletepost","删除回复"},
            { "deletethread","删除帖子"},
            { "deleteuser","删除用户"},
            { "controluser","修改用户"},
            { "getcoins","获得金币"},
            { "getprestige","获得威望"},
            { "getpoint","获得积分"},
            { "uploadfile","上传文件"},
            { "uploadimage","上传图片"},
            { "download","下载附件"},
            { "mainpage","主页"},
            { "search","搜索"},
            { "apply","修改个人信息"},
            { "game","参加有趣的冒险"}

        };


        public static Dictionary<string, UserConfig> Users = new Dictionary<string, UserConfig>();

        public static Dictionary<int, List<int>> AreaThreads = new Dictionary<int, List<int>>() {
            { 1,new List<int>()},
            { 2,new List<int>()},
            { 3,new List<int>()}
        };

        public static Dictionary<int, string> AreaNote = new Dictionary<int, string>() {
            { 1,"发布网站公告的地方！禁止发布其他内容，违者删帖封号等处罚。"},
            { 2,"闲聊区，快来水贴吧！"},
            { 3,"发布与反馈"}

        };
        public static Dictionary<int, string> AreaName = new Dictionary<int, string>()
        {
            { 1,"网站公告"},
            { 2,"水贴区"},
            { 3,"STAmxxHelper讨论区"}
        };

        public static Dictionary<int, string> Blocks = new Dictionary<int, string>() {
            { 1,"核心区域"},
            { 2,"核心区域"},
            { 3,"第二区域"}

        };

        public static Dictionary<int, PostThread> Threads = new Dictionary<int, PostThread>();

        public static Dictionary<int, PostReply> Replys = new Dictionary<int, PostReply>();

        public static Dictionary<int, List<int>> ThreadReplys = new Dictionary<int, List<int>>();

        public static List<string> Admins = new List<string>() {
            "SpaceTimeX","sogouwap"
        };

        public static List<string> Emails = new List<string>();

        public static void checkPoint(string username)
        {
            string nowrank = Users[username].Rank;
            if (!iConf.Prefixes.Contains(nowrank)) return;
            long point = Users[username].Point;
            int i = iConf.GetNowpos(point);
            
            Users[username].Rank = iConf.Prefixes[i];
        }
    }

    public static class iConf
    {
        public static List<long> LevelPoints = new List<long>() { 0, 100, 500, 1000, 5000, 10000, 50000, 100000, 2100000000 };
        public static List<string> Prefixes = new List<string>() { "空气", "信徒", "传教士", "教主", "教皇", "时官", "时臣", "诸神", "时空之主" };

        public static long GetLeastPoints(long now)
        {
            long l = -1;
            for (int i = 0; i < LevelPoints.Count; i++)
            {
                if (LevelPoints[i] > now)
                {
                    l = LevelPoints[i] - now;
                    break;
                }
            }

            return l;
        }

        public static int GetNowpos(long now)
        {
            for (int i = 0; i < LevelPoints.Count; i++)
            {
                if (LevelPoints[i] > now)
                {
                    return i;
                }
            }

            return 0;

        }

    }
}
