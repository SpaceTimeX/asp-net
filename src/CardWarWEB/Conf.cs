using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardWarWEB
{
    public class Conf
    {
        public static IHostingEnvironment env;
        public static IApplicationEnvironment appEnv;
        public static Dictionary<string, Dictionary<string, string>> UserConf = new Dictionary<string, Dictionary<string, string>>();
    }
}
