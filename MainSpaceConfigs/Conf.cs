using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardWarWEB
{
    [Serializable]
    public class UserConfig
    {
        public string Username;
        public string Password;
        public string Email;
        public List<string> Flags = new List<string>() { "post", "thread", "home", "login", "resetpassword", "resetemail",
                                   "view", "message", "styletext","getcoins","getprestige","getpoint","sendemail","sign","mainpage","apply","uploadimage","game" };
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
        public bool IsGameActivited = false;
        public iGame iG;
    }


    [Serializable]
    public class iGame
    {
        public string Username;
        public string Nick;
        public iClasses Class;
        public Achievement Achieve = new Achievement();
        public Dictionary<int, iItems> Items = new Dictionary<int, iItems>();
        public iPro Pro;
        public iPro OriginalPro;
        public iTypes Type = iTypes.普通;
        public long Coin = 0;
        public long Level = 1;
        public long Exp = 0;
    }

    [Serializable]
    public enum iClasses
    {
        战士,
        法师,
        刺客,
        骑士

    };

    [Serializable]
    public enum iTypes
    {
        普通,
        精英,
        王者,
        首领,
        帝皇,
        主宰,
        史诗,
        传说,
        半神,
        真神,
        不朽
    };

    [Serializable]
    public class iItems
    {
        public string ItemName;

    }

    [Serializable]
    public class iPro
    {
        public double HP;
        public double MP;
        public double AD;
        public double AP;
        public double AR;

        public double EX = 0;
        public double PT = 0;

        public long LL = 0; //力量
        public long ZL = 0; //智力
        public long NL = 0; //耐力
        public long MJ = 0; //敏捷
        public long SB = 0; //闪避
        public long JZ = 0; //精准

        public long UsedPoint = 0;
        public long NonUsedPoint = 5;
    }

    [Serializable]
    public class Message
    {
        public string FromUsername;
        public string SendTime;
        public string Title;
        public string Content;
        public bool IsRead = false;
    }

    [Serializable]
    public class WebConfig
    {
        public Activity Active = new Activity();
    }

    public class AttackingPro
    {

        public double HP;
        public double MP;
        public double AD;
        public double AP;
        public double AR;
        public double ARM;

        public double EX;
        public double PT;
        public double MISS;
        public double BOOM;
    }

    public class EvalResult
    {
        public bool isSuccess = false;
        public string ErrorMessage;
        public object ReturnResult;


    }

    public class Conf {

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


        public static Dictionary<string, UserConfig> Users = new Dictionary<string, UserConfig>() {
            { "SpaceTimeX" ,new UserConfig() { Username = "SpaceTimeX", Password="C6C115BE997E4C0C50A80C856757724F",Email = "1250164276@qq.com" } }

        };

        public static WebConfig WebConfigs = new WebConfig();

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

    public class GiveResult
    {
        public long GetEXP = 0;
        public bool IsLevelUp = false;
        public long LevelUpCount = 0;

    }

    public static class iConf
    {
        public static List<long> LevelPoints = new List<long>() { 0, 100, 500, 1000, 5000, 10000, 50000, 100000, 2100000000 };
        public static List<string> Prefixes = new List<string>() { "空气", "信徒", "传教士", "教主", "教皇", "时官", "时臣", "诸神", "时空之主" };
        public static List<long> LevelExps = new List<long>() { 0, 100, 200, 300, 500, 700, 900, 1100, 1300, 1500, 2000, 2500, 3000, 3500, 4000, 5000, 6000, 7000, 8000, 9000, 15000, 20000, 25000, 30000, 35000, 45000, 55000, 65000, 75000, 85000, 100000, 120000, 140000, 160000, 180000, 210000, 240000, 270000, 300000, 330000, 380000, 430000, 480000, 530000, 580000, 700000, 800000, 900000, 1000000, 1100000 };
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

        public static long GetLeastExps(long level)
        {
            return LevelExps[Convert.ToInt32(level)];
        }

        public static GiveResult GiveEXP(string username, long num)
        {
            iGame ig = Conf.Users[username].iG;
            long now = ig.Exp;
            long level = ig.Level;
            long least = GetLeastExps(level);
            int i = 0;
            GiveResult gr = new GiveResult();
            gr.GetEXP = num;
            if (num > least)
            {
                i++;
                num -= least;
                now = 0;
                least = GetLeastExps(level + i);
            }
            else {
                now += num;
                num = 0;
            }

            while (num > 0)
            {
                least = GetLeastExps(level + i);
                if (num >= least)
                {
                    num -= least;
                    i++;
                }
                else if ((num < least) && (num > 0))
                {
                    now = num;
                    num = 0;
                }

            }

            ig.Exp = now;
            ig.Level += i;
            ig.Pro.NonUsedPoint += 5 * i;
            if (i > 0)
                gr.IsLevelUp = true;
            gr.LevelUpCount = i;
            return gr;

        }


        public static AttackingPro GetAttackingPro(iGame data)
        {

            long a, b, c, d, e;
            a = data.Pro.LL;
            b = data.Pro.ZL;
            c = data.Pro.NL;
            d = data.Pro.SB;
            e = data.Pro.JZ;
            double hp, mp, ad, ap, ar, ex, level, arm, miss, boom, pt;
            hp = data.Pro.HP;
            mp = data.Pro.MP;
            ad = data.Pro.AD;
            ap = data.Pro.AP;
            ar = data.Pro.AR;
            ex = data.Pro.EX;
            pt = data.Pro.PT;
            level = data.Level;
            hp += 12 * a + 18 * c + 18 * d;
            mp += 16 * b + 22 * c + 22 * d;
            ad += 3 * a + 3 * e;
            ap += 5 * b + 4 * e;
            ar += 0.007 * c;
            arm = iGetARM(ar);
            miss = iGetMISS(d);
            boom = iGetBOOM(a, b, e);
            AttackingPro Atp = new AttackingPro() { AD = ad, AP = ap, AR = ar, ARM = arm, BOOM = boom, EX = ex, HP = hp, MISS = miss, MP = mp, PT = pt };
            return Atp;
        }

        public static double iGetARM(double ar)
        {
            var t = ((double)ar / (100d + (double)ar));
            return t;
        }


        public static double iGetMISS(long sb)
        {
            var t = sb * 0.00004;
            return t;
        }

        public static double iGetBOOM(long ll, long zl, long jz)
        {
            var t = jz * 0.00002 + ll * 0.000006 + zl * 0.000006;
            return t;
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
