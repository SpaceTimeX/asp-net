using Microsoft.AspNet.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardWarWEB
{
    public class Server
    {
        public static string MapPath(string path)
        {
            return Conf.env.MapPath(path);

        }

        public static string GetApplicationBasePath()
        {
            return Conf.appEnv.ApplicationBasePath;
        }
    }
}
