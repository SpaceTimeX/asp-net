using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.StaticFiles;
using Microsoft.AspNet.FileProviders;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using System.Runtime.InteropServices;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Security.Cryptography;
using System.Text;

namespace CardWarWEB
{


    public class Startup
    {

        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            localConf.env = env;
            localConf.appEnv = appEnv;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }


        public void Configure(IApplicationBuilder app)
        {

            Dictionary<string, string> mappings = new Dictionary<string, string>() {
                { ".txt", "text/plain" },
                { ".css", "text/css" },
                { ".html","text/html"},
                { ".htm","text/html"},
                { ".jpeg","image/jpeg"},
                { ".jpg","image/jpeg" },
                { ".wav ","audio/x-wav"},
                { ".js","text/javascript" },
                { ".svg","image/svg+xml"},
                { ".woff","application/x-font-woff"},
                { ".woff2","application/x-font-woff"},
                { ".eot" ,"application/vnd.ms-fontobject"},
                { ".apk","application/vnd.android.package-archive"},
                { ".ipa","application/iphone"},
                { ".mp3","audio/mpeg"},
                { ".gif","image/gif"},
                { ".png", "image/png"},
                { ".swf","application/x-shockwave-flash"},
                { ".rar","application/octet-stream"},
                { ".7z","application/octet-stream"},
                { ".zip","application/zip"},
                { ".ico","application/octet-stream"},
                { ".map","application/octet-stream"}

            };

            FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider(mappings);
            PhysicalFileProvider physicalprovider = new PhysicalFileProvider(Server.GetApplicationBasePath() + "/sRaw");
            app.UseStaticFiles(new StaticFileOptions() { ServeUnknownFileTypes = false, ContentTypeProvider = provider, RequestPath = "/sRaw", FileProvider = physicalprovider });
            app.UseStatusCodePages(status =>
            {
                bool over = false;
                status.Run(context =>
                {
                    if (over)
                    {
                        return Task.Delay(100);
                    }
                    over = true;
                    return Task.Run(delegate { context.Response.Redirect(context.Request.PathBase + "/Error/" + context.Response.StatusCode.ToString()); over = false; });


                });

            });
            app.UseDeveloperExceptionPage();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}",
                    defaults: new { controller = "Welcome", action = "Index" });

            });

            if (Directory.Exists("/v"))
            {
                if (File.Exists("/v/WebConfigs.data"))
                {
                    FileStream fs = new FileStream("/v/WebConfigs.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    Conf.WebConfigs = (WebConfig)bf.Deserialize(fs);
                    fs.Close();
                }
                else {
                    FileStream fs = new FileStream("/v/WebConfigs.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, Conf.WebConfigs);
                    fs.Close();
                }

                if (File.Exists("/v/AreaThreads.data"))
                {
                    FileStream fs = new FileStream("/v/AreaThreads.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    Conf.AreaThreads = (Dictionary<int, List<int>>)bf.Deserialize(fs);
                    fs.Close();
                }
                else {
                    FileStream fs = new FileStream("/v/AreaThreads.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, Conf.AreaThreads);
                    fs.Close();
                }

                if (File.Exists("/v/Users.data"))
                {
                    FileStream fs = new FileStream("/v/Users.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    Conf.Users = (Dictionary<string, UserConfig>)bf.Deserialize(fs);
                    fs.Close();
                }
                else {
                    FileStream fs = new FileStream("/v/Users.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, Conf.Users);
                    fs.Close();
                }

                if (File.Exists("/v/Threads.data"))
                {
                    FileStream fs = new FileStream("/v/Threads.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    Conf.Threads = (Dictionary<int, PostThread>)bf.Deserialize(fs);
                    fs.Close();
                }
                else {
                    FileStream fs = new FileStream("/v/Threads.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, Conf.Threads);
                    fs.Close();
                }

                if (File.Exists("/v/Replys.data"))
                {
                    FileStream fs = new FileStream("/v/Replys.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    Conf.Replys = (Dictionary<int, PostReply>)bf.Deserialize(fs);
                    fs.Close();
                }
                else {
                    FileStream fs = new FileStream("/v/Replys.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, Conf.Replys);
                    fs.Close();
                }

                if (File.Exists("/v/ThreadReplys.data"))
                {
                    FileStream fs = new FileStream("/v/ThreadReplys.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    Conf.ThreadReplys = (Dictionary<int, List<int>>)bf.Deserialize(fs);
                    fs.Close();
                }
                else {
                    FileStream fs = new FileStream("/v/ThreadReplys.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, Conf.ThreadReplys);
                    fs.Close();
                }

                if (File.Exists("/v/Emails.data"))
                {
                    FileStream fs = new FileStream("/v/Emails.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    Conf.Emails = (List<string>)bf.Deserialize(fs);
                    fs.Close();
                }
                else {
                    FileStream fs = new FileStream("/v/Emails.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, Conf.Emails);
                    fs.Close();
                }
            }
            else
            {
                if (File.Exists(Server.GetApplicationBasePath() + "/WebConfigs.data"))
                {
                    FileStream fs = new FileStream(Server.GetApplicationBasePath() + "/WebConfigs.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    Conf.WebConfigs = (WebConfig)bf.Deserialize(fs);
                    fs.Close();
                }
                else {
                    FileStream fs = new FileStream(Server.GetApplicationBasePath() + "/WebConfigs.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, Conf.WebConfigs);
                    fs.Close();
                }

                if (File.Exists(Server.GetApplicationBasePath() + "/AreaThreads.data"))
                {
                    FileStream fs = new FileStream(Server.GetApplicationBasePath() + "/AreaThreads.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    Conf.AreaThreads = (Dictionary<int, List<int>>)bf.Deserialize(fs);
                    fs.Close();
                }
                else {
                    FileStream fs = new FileStream(Server.GetApplicationBasePath() + "/AreaThreads.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, Conf.AreaThreads);
                    fs.Close();
                }

                if (File.Exists(Server.GetApplicationBasePath() + "/Users.data"))
                {
                    FileStream fs = new FileStream(Server.GetApplicationBasePath() + "/Users.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    Conf.Users = (Dictionary<string, UserConfig>)bf.Deserialize(fs);
                    fs.Close();
                }
                else {
                    FileStream fs = new FileStream(Server.GetApplicationBasePath() + "/Users.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, Conf.Users);
                    fs.Close();
                }

                if (File.Exists(Server.GetApplicationBasePath() + "/Threads.data"))
                {
                    FileStream fs = new FileStream(Server.GetApplicationBasePath() + "/Threads.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    Conf.Threads = (Dictionary<int, PostThread>)bf.Deserialize(fs);
                    fs.Close();
                }
                else {
                    FileStream fs = new FileStream(Server.GetApplicationBasePath() + "/Threads.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, Conf.Threads);
                    fs.Close();
                }


                if (File.Exists(Server.GetApplicationBasePath() + "/Replys.data"))
                {
                    FileStream fs = new FileStream(Server.GetApplicationBasePath() + "/Replys.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    Conf.Replys = (Dictionary<int, PostReply>)bf.Deserialize(fs);
                    fs.Close();
                }
                else {
                    FileStream fs = new FileStream(Server.GetApplicationBasePath() + "/Replys.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, Conf.Replys);
                    fs.Close();
                }

                if (File.Exists(Server.GetApplicationBasePath() + "/ThreadReplys.data"))
                {
                    FileStream fs = new FileStream(Server.GetApplicationBasePath() + "/ThreadReplys.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    Conf.ThreadReplys = (Dictionary<int, List<int>>)bf.Deserialize(fs);
                    fs.Close();
                }
                else {
                    FileStream fs = new FileStream(Server.GetApplicationBasePath() + "/ThreadReplys.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, Conf.ThreadReplys);
                    fs.Close();
                }


                if (File.Exists(Server.GetApplicationBasePath() + "/Emails.data"))
                {
                    FileStream fs = new FileStream(Server.GetApplicationBasePath() + "/Emails.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    Conf.Emails = (List<string>)bf.Deserialize(fs);
                    fs.Close();
                }
                else {
                    FileStream fs = new FileStream(Server.GetApplicationBasePath() + "/Emails.data", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, Conf.Emails);
                    fs.Close();
                }

            }

        }
    }
}
