using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Http.Features;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Http;
using Microsoft.Net.Http.Headers;
using System.Net;
using System.Collections;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CardWarWEB.Controllers
{
    [Route("Editor")]
    public class EditorController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
        [Route("Um")]
        public IActionResult Um()
        {
            return View();
        }



        UploadConfig UploadConfig = new UploadConfig();
        UploadResult Result = new UploadResult();

        private int Start;
        private int Size;
        private int Total;
        private ResultState State;
        private string PathToList;
        private string[] FileList;
        private string[] SearchExtensions;

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

        [Route("Editor")]
        public object Editor()
        {
            if (!Request.Query.ContainsKey("action"))
            {
                return "Error";
            }

            if (!CheckLogin()) {

                Server.RemoveCookie(this, "username");
                Server.RemoveCookie(this, "password");
                return "Error";
            }
            string username = Request.Cookies["username"];
            List<string> flags = Conf.Users[username].Flags;
            string R = Request.Query["action"];
            if (R == "config")
            {
                return Config.Items.ToString();
            }
            else if (R == "uploadimage" || R == "uploadscrawl" || R == "uploadvideo" || R == "uploadfile")
            {
                if (R == "uploadimage")
                {
                    UploadConfig = new UploadConfig()
                    {
                        AllowExtensions = Config.GetStringList("imageAllowFiles"),
                        PathFormat = Config.GetString("imagePathFormat"),
                        SizeLimit = Config.GetInt("imageMaxSize"),
                        UploadFieldName = Config.GetString("imageFieldName")
                    };
                }
                else if (R == "uploadscrawl")
                {
                    UploadConfig = new UploadConfig()
                    {
                        AllowExtensions = new string[] { ".png" },
                        PathFormat = Config.GetString("scrawlPathFormat"),
                        SizeLimit = Config.GetInt("scrawlMaxSize"),
                        UploadFieldName = Config.GetString("scrawlFieldName"),
                        Base64 = true,
                        Base64Filename = "scrawl.png"
                    };
                }
                else if (R == "uploadvideo")
                {
                    UploadConfig = new UploadConfig()
                    {
                        AllowExtensions = Config.GetStringList("videoAllowFiles"),
                        PathFormat = Config.GetString("videoPathFormat"),
                        SizeLimit = Config.GetInt("videoMaxSize"),
                        UploadFieldName = Config.GetString("videoFieldName")
                    };
                }
                else if (R == "uploadfile") {
                    UploadConfig = new UploadConfig()
                    {
                        AllowExtensions = Config.GetStringList("fileAllowFiles"),
                        PathFormat = Config.GetString("filePathFormat"),
                        SizeLimit = Config.GetInt("fileMaxSize"),
                        UploadFieldName = Config.GetString("fileFieldName")
                    };

                }

                string uploadFileName = null;

                var file = Request.Form.Files[UploadConfig.UploadFieldName];
                if (file == null) return "";
                var parsedContentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);

                var originalName = parsedContentDisposition.FileName.Replace("\"", "");
                uploadFileName = originalName;
                if (R == "uploadfile")
                {
                    if (!flags.Contains("uploadfile"))
                    {
                        Result.State = UploadState.NoFileFlag;
                        WriteResult(Result);
                        return "";
                    }
                }
                if (R == "uploadimage")
                {
                    if (!flags.Contains("uploadimage"))
                    {
                        Result.State = UploadState.NoImageFlag;
                        WriteResult(Result);
                        return "";
                    }
                }
                if (!CheckFileType(uploadFileName))
                {
                    Result.State = UploadState.TypeNotAllow;
                    WriteResult(Result);
                    return "";
                }
                if (!CheckFileSize(Convert.ToInt32(file.Length)))
                {
                    Result.State = UploadState.SizeLimitExceed;
                    WriteResult(Result);
                    return "";
                }
                Result.OriginFileName = uploadFileName;
                var savePath = PathFormatter.Format(uploadFileName, UploadConfig.PathFormat);
                var localPath = Server.MapPath(savePath);

                if (UploadConfig.Base64)
                {
                    uploadFileName = UploadConfig.Base64Filename;
                }

                try
                {
                    if (!Directory.Exists(Path.GetDirectoryName(localPath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(localPath));
                    }
                    file.SaveAs(localPath);
                    Result.Url = savePath;
                    Result.State = UploadState.Success;
                }
                catch (Exception e)
                {
                    Result.State = UploadState.FileAccessError;
                    Result.ErrorMessage = e.Message;
                    WriteResult(Result);
                }
                finally
                {
                    WriteResult(Result);
                }

            }
            else if (R == "listfile" || R == "listimage")
            {
                string[] searchExtensions = null;
                if (R == "listfile")
                {
                    searchExtensions = Config.GetStringList("fileManagerAllowFiles");
                    this.SearchExtensions = searchExtensions.Select(x => x.ToLower()).ToArray();
                    this.PathToList = Config.GetString("fileManagerListPath");
                } else if(R == "listimage")
                {
                    searchExtensions = Config.GetStringList("imageManagerAllowFiles");
                    this.SearchExtensions = searchExtensions.Select(x => x.ToLower()).ToArray();
                    this.PathToList = Config.GetString("imageManagerListPath");
                }

                try
                {
                    Start = string.IsNullOrEmpty(Request.Query["start"]) ? 0 : Convert.ToInt32(Request.Query["start"]);
                    Size = string.IsNullOrEmpty(Request.Query["size"]) ? Config.GetInt("imageManagerListSize") : Convert.ToInt32(Request.Query["size"]);
                }
                catch (FormatException)
                {
                    State = ResultState.InvalidParam;
                    WriteResult_ListFile();
                    return "";
                }
                var buildingList = new List<string>();
                try
                {
                    var localPath = Server.MapPath(PathToList);
                    buildingList.AddRange(Directory.GetFiles(localPath, "*", SearchOption.AllDirectories)
                        .Where(x => SearchExtensions.Contains(Path.GetExtension(x).ToLower()))
                        .Select(x => PathToList + x.Substring(localPath.Length).Replace("\\", "/")));
                    Total = buildingList.Count;
                    FileList = buildingList.OrderBy(x => x).Skip(Start).Take(Size).ToArray();
                }
                catch (UnauthorizedAccessException)
                {
                    State = ResultState.AuthorizError;
                }
                catch (DirectoryNotFoundException)
                {
                    State = ResultState.PathNotFound;
                }
                catch (IOException)
                {
                    State = ResultState.IOError;
                }
                finally
                {
                    WriteResult_ListFile();
                }
            }
            else if (R == "catchimage")
            {
                string[] Sources;
                Crawler[] Crawlers;
                Sources = Request.Form["source[]"];
                if (Sources == null || Sources.Length == 0)
                {


                    Dictionary<string, object> sis = new Dictionary<string, object>()
                    {
                        { "state" , "参数错误：没有指定抓取源"}
                    };

                    HttpContext.Response.Clear();
                    string jjson = JsonConvert.SerializeObject(sis);
                    HttpContext.Response.WriteAsync(jjson);
                    return "";
                }
                Crawlers = Sources.Select(x => new Crawler(x).Fetch()).ToArray();
                Dictionary<string, object> ss = new Dictionary<string, object>()
                {
                    { "state" , "SUCCESS"},
                    { "list" , Crawlers.Select(x => new
                        {
                            state = x.State,
                            source = x.SourceUrl,
                            url = x.ServerUrl
                        })
                    }
                };

                HttpContext.Response.Clear();
                string json = JsonConvert.SerializeObject(ss);
                HttpContext.Response.WriteAsync(json);
            }
            else
            {
                Dictionary<string, object> ss = new Dictionary<string, object>()
                {
                    { "state" , "action 参数为空或者 action 不被支持。"}
                };

                HttpContext.Response.Clear();
                string json = JsonConvert.SerializeObject(ss);
                HttpContext.Response.WriteAsync(json);

            }

            return "";
        }

        [Route("Upload")]
        public void Upload()
        {
            //上传配置
            string pathbase = "upload/";                                                          //保存路径
            int size = 10;                     //文件大小限制,单位mb                                                                                   //文件大小限制，单位KB
            string[] filetype = { ".gif", ".png", ".jpg", ".jpeg", ".bmp" };                    //文件允许格式

            string callback = HttpContext.Request.Query["callback"];
            string editorId = HttpContext.Request.Query["editorid"];

            //上传图片
            Hashtable info;
            Uploader up = new Uploader();
            info = up.upFile(HttpContext, pathbase, filetype, size); //获取上传状态
            string json = BuildJson(info);

            HttpContext.Response.ContentType = "text/html";
            if (callback != null)
            {
                HttpContext.Response.WriteAsync(string.Format("<script>{0}(JSON.parse(\"{1}\"));</script>", callback, json));
            }
            else
            {
                HttpContext.Response.WriteAsync(json);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private string BuildJson(Hashtable info)
        {
            List<string> fields = new List<string>();
            string[] keys = new string[] { "originalName", "name", "url", "size", "state", "type" };
            for (int i = 0; i < keys.Length; i++)
            {
                fields.Add(string.Format("\"{0}\": \"{1}\"", keys[i], info[keys[i]]));
            }
            return "{" + string.Join(",", fields) + "}";
        }

        private void WriteResult_ListFile()
        {

            Dictionary<string, object> ss = new Dictionary<string, object>()
            {
                { "state" , GetStatestring()},
                { "list" , FileList == null ? null : FileList.Select(x => new { url = x }) },
                { "start" , Start },
                { "size" ,Size },
                { "total" ,Total }
            };

            HttpContext.Response.Clear();
            string json = JsonConvert.SerializeObject(ss);
            HttpContext.Response.WriteAsync(json);
        }

        enum ResultState
        {
            Success,
            InvalidParam,
            AuthorizError,
            IOError,
            PathNotFound
        }

        private string GetStatestring()
        {
            switch (State)
            {
                case ResultState.Success:
                    return "SUCCESS";
                case ResultState.InvalidParam:
                    return "参数不正确";
                case ResultState.PathNotFound:
                    return "路径不存在";
                case ResultState.AuthorizError:
                    return "文件系统权限不足";
                case ResultState.IOError:
                    return "文件系统读取错误";
            }
            return "未知错误";
        }

        private void WriteResult(UploadResult uprs)
        {
            Dictionary<string, string> ss = new Dictionary<string, string>()
            {
                { "state" , GetStateMessage(uprs.State)},
                { "url" , uprs.Url },
                { "title" , uprs.OriginFileName },
                { "original" , uprs.OriginFileName },
                { "error" ,uprs.ErrorMessage }
            };

            HttpContext.Response.Clear();
            string json = JsonConvert.SerializeObject(ss);
            HttpContext.Response.WriteAsync(json);
        }

        private string GetStateMessage(UploadState state)
        {
            switch (state)
            {
                case UploadState.Success:
                    return "SUCCESS";
                case UploadState.FileAccessError:
                    return "文件访问出错，请检查写入权限";
                case UploadState.SizeLimitExceed:
                    return "文件大小超出服务器限制";
                case UploadState.TypeNotAllow:
                    return "不允许的文件格式";
                case UploadState.NetworkError:
                    return "网络错误";
                case UploadState.NoFileFlag:
                    return "没有上传文件权限！";
                case UploadState.NoImageFlag:
                    return "没有上传图片权限！";
            }
            return "未知错误";
        }

        private bool CheckFileType(string filename)
        {
            var fileExtension = Path.GetExtension(filename).ToLower();
            return UploadConfig.AllowExtensions.Select(x => x.ToLower()).Contains(fileExtension);
        }

        private bool CheckFileSize(int size)
        {
            return size < UploadConfig.SizeLimit;
        }
    }

    public class UploadConfig
    {
        /// <summary>
        /// 文件命名规则
        /// </summary>
        public string PathFormat { get; set; }

        /// <summary>
        /// 上传表单域名称
        /// </summary>
        public string UploadFieldName { get; set; }

        /// <summary>
        /// 上传大小限制
        /// </summary>
        public int SizeLimit { get; set; }

        /// <summary>
        /// 上传允许的文件格式
        /// </summary>
        public string[] AllowExtensions { get; set; }

        /// <summary>
        /// 文件是否以 Base64 的形式上传
        /// </summary>
        public bool Base64 { get; set; }

        /// <summary>
        /// Base64 字符串所表示的文件名
        /// </summary>
        public string Base64Filename { get; set; }
    }

    public class UploadResult
    {
        public UploadState State { get; set; }
        public string Url { get; set; }
        public string OriginFileName { get; set; }

        public string ErrorMessage { get; set; }
    }

    public enum UploadState
    {
        Success = 0,
        SizeLimitExceed = -1,
        TypeNotAllow = -2,
        FileAccessError = -3,
        NetworkError = -4,
        Unknown = 1,
        NoFileFlag = -5,
        NoImageFlag = -6
    }

    public class Crawler
    {
        public string SourceUrl { get; set; }
        public string ServerUrl { get; set; }
        public string State { get; set; }



        public Crawler(string sourceUrl)
        {
            this.SourceUrl = sourceUrl;
        }

        public Crawler Fetch()
        {
            if (!IsExternalIPAddress(this.SourceUrl))
            {
                State = "INVALID_URL";
                return this;
            }
            var request = HttpWebRequest.Create(this.SourceUrl) as HttpWebRequest;
            using (var response = request.GetResponseAsync().Result as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    State = "Url returns " + response.StatusCode + ", " + response.StatusDescription;
                    return this;
                }
                if (response.ContentType.IndexOf("image") == -1)
                {
                    State = "Url is not an image";
                    return this;
                }
                ServerUrl = PathFormatter.Format(Path.GetFileName(this.SourceUrl), Config.GetString("catcherPathFormat"));
                var savePath = Server.MapPath(ServerUrl);
                if (!Directory.Exists(Path.GetDirectoryName(savePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                }
                try
                {
                    var stream = response.GetResponseStream();
                    var reader = new BinaryReader(stream);
                    byte[] bytes;
                    using (var ms = new MemoryStream())
                    {
                        byte[] buffer = new byte[4096];
                        int count;
                        while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            ms.Write(buffer, 0, count);
                        }
                        bytes = ms.ToArray();
                    }
                    File.WriteAllBytes(savePath, bytes);
                    stream.Close();
                    reader.Close();
                    State = "SUCCESS";
                }
                catch (Exception e)
                {
                    State = "抓取错误：" + e.Message;
                }
                return this;
            }
        }

        private bool IsExternalIPAddress(string url)
        {
            var uri = new Uri(url);
            switch (uri.HostNameType)
            {
                case UriHostNameType.Dns:
                    var ipHostEntry = Dns.GetHostEntryAsync(uri.DnsSafeHost).Result;
                    foreach (IPAddress ipAddress in ipHostEntry.AddressList)
                    {
                        byte[] ipBytes = ipAddress.GetAddressBytes();
                        if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            if (!IsPrivateIP(ipAddress))
                            {
                                return true;
                            }
                        }
                    }
                    break;

                case UriHostNameType.IPv4:
                    return !IsPrivateIP(IPAddress.Parse(uri.DnsSafeHost));
            }
            return false;
        }

        private bool IsPrivateIP(IPAddress myIPAddress)
        {
            if (IPAddress.IsLoopback(myIPAddress)) return true;
            if (myIPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                byte[] ipBytes = myIPAddress.GetAddressBytes();
                // 10.0.0.0/24 
                if (ipBytes[0] == 10)
                {
                    return true;
                }
                // 172.16.0.0/16
                else if (ipBytes[0] == 172 && ipBytes[1] == 16)
                {
                    return true;
                }
                // 192.168.0.0/16
                else if (ipBytes[0] == 192 && ipBytes[1] == 168)
                {
                    return true;
                }
                // 169.254.0.0/16
                else if (ipBytes[0] == 169 && ipBytes[1] == 254)
                {
                    return true;
                }
            }
            return false;
        }
    }
}















