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

namespace CardWarWEB
{
    public class Startup
    {

        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            Conf.env = env;
            Conf.appEnv = appEnv;
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
                { ".ico","application/octet-stream"}

            };

            FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider(mappings);

            app.UseStaticFiles(new StaticFileOptions() { ServeUnknownFileTypes = false, ContentTypeProvider = provider, RequestPath = "/sRaw" });

            DirectoryBrowserOptions diroptions = new DirectoryBrowserOptions();
            diroptions.RequestPath = "/sRaw";
            app.UseDirectoryBrowser(diroptions);
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
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Welcome", action = "Index" });

            });

        }
    }
}
