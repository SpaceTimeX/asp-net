using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardWarWEB
{
    [Serializable]
    public class Activity
    {
        public string Name = "火元素";
        public iTypes Type = iTypes.精英;
        public long Level = 30;
        public long HP = 2400000000;
        public long NOWHP = 2400000000;
        public long ATK = 38;
        public double GiveExpPercent = 0.2;
        public long GiveExp = 3;
    }
}
